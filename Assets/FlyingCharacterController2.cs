using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FlyingCharacterController2 : MonoBehaviour {

    public enum State
    {
        Grounded,
        Flying,
        Fallen,
        Airborne
    }

    private IEnumerator rotateCoroutine;

    [Header("State")]
    public State currentState;

    [Header("Components")]
    public Animator myAnimator;
    CharacterController cc;
    Transform cameraT;
    public BehaviourPuppet puppet { get; private set; }
    public PuppetMaster myPuppetMaster;

    [Header("Controls")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpButton = "Jump";
    public string flyToggle = "FlyToggle";

    [Header("Grounded Speeds and Numeric Stuff")]
    public float runSpeed = 6;
    public float walkSpeed = 2;
    public float turnSmoothTime = 0.2f;
    public float speedSmoothTime = 0.1f;
    public float gravity = -9.8f;
    public float jumpHeight;
    public float fallRate = -1.5f;
    public float jumpScaler = 5f;
    public float landingDelay = 1.5f;
    public float ySpeed;
    public float angularDrag;
    float speedSmoothVelocity;
    float currentSpeed;
    float turnSmoothVelocity;

    [Header("Flying Speeds and Numeric Stuff")]
    public float baseFlySpeed;
    public float flySpeed;
    public float rotResetSpeed;
    public float flapBonus;
    public float angleMultiplier;
    public int flaps;

    // Use this for initialization
    void Start () {
        myAnimator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        puppet = transform.parent.GetComponentInChildren<BehaviourPuppet>();
        myPuppetMaster = transform.parent.GetComponentInChildren<PuppetMaster>();
        cameraT = Camera.main.transform;

        flySpeed = baseFlySpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Grounded:
                Grounded();
                break;

            case State.Flying:
                Flying();
                break;

            case State.Fallen:
                Fallen();
                break;

            case State.Airborne:
                Airborne();
                break;
        }
    }

    void Grounded()
    {
        if (puppet.state != BehaviourPuppet.State.Puppet) return;
        //Basic Movement controller adapted from Sebastian Lague's Character Creation Tutorials

        if(myAnimator.GetBool("Grounded") == false)
        {
            myAnimator.SetBool("Grounded", true);
            myAnimator.SetTrigger("Landing");
        }

        Vector2 input = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float mag = Mathf.Clamp01(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude);

        float speed = runSpeed * inputDir.magnitude * mag * Time.deltaTime;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref speedSmoothVelocity, speedSmoothTime);

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * ySpeed;

        Debug.DrawRay(transform.position, Vector3.down * .75f, Color.magenta);

        //Jumping and Falling
        if (cc.isGrounded)
        {
            if (Input.GetButtonDown(jumpButton))
            {
                if (myAnimator.GetBool("Grounded"))
                {
                    myAnimator.SetTrigger("Jumping");
                }
                ySpeed = jumpHeight;
                currentState = State.Airborne;
            }
            
        }
        else
        {
            RaycastHit hit;
            

            if(Physics.Raycast(transform.position, Vector3.down * .75f, out hit))
            {
                if (!hit.collider.gameObject.CompareTag("Ground"))
                {
                    currentState = State.Airborne;
                }
                else
                {
                    ySpeed += gravity * Time.deltaTime;
                    
                    if(ySpeed > gravity)
                    {
                        ySpeed = gravity;
                    }
                }
            }
            
        }

        velocity.y = ySpeed;

        cc.Move(velocity * Time.deltaTime);

        if (cc.isGrounded)
        {
            float animSpeedPercent = mag;
            myAnimator.SetFloat("Move", animSpeedPercent, speedSmoothTime, Time.deltaTime);
        }
    }

    void Flying()
    {
        //Base flight controls 
        if (puppet.state != BehaviourPuppet.State.Puppet) return;

        //myPuppetMaster.mode = PuppetMaster.Mode.Kinematic;

        myAnimator.SetBool("Grounded", false);

        float flyingAnimDelay;

        flyingAnimDelay = Mathf.Lerp(myAnimator.GetFloat("Flying"), 0f, 1.0f);

        transform.position += transform.forward * Time.deltaTime * flySpeed * 2f;

        flySpeed -= transform.forward.y * Time.deltaTime * angleMultiplier;

        if (Input.GetButtonDown(jumpButton) && flaps > 0)
        {
            flySpeed += flapBonus;
            flaps -= 1;
        }

        if(flySpeed < 1.8)
        {
            myAnimator.SetFloat("Flying", flySpeed);
        }
        else
        {
            myAnimator.SetFloat("Flying", 1.8f);
        }

        if (flySpeed < 0)
        {
            StartCoroutine(RotateMe());
            myAnimator.SetFloat("Flying", 0f);
            currentState = State.Airborne;
        }

        transform.Rotate(Input.GetAxis(verticalAxis), 0.0f, -1.0f * (Input.GetAxis(horizontalAxis)));

        if (Input.GetButtonDown(flyToggle))
        {
            
            Debug.Log(cc.isGrounded);
            myAnimator.SetFloat("Flying", 0f);
            flySpeed = baseFlySpeed;
            if (cc.isGrounded)
            {
                currentState = State.Grounded;
            }
            else
            {
                currentState = State.Airborne;
            }
            StartCoroutine(RotateMe());
        }


    }

    void Fallen()
    {

    }

    void Airborne()
    {
        Vector3 velocity = (transform.forward * currentSpeed * angularDrag) + Vector3.up * ySpeed;

        myAnimator.SetFloat("Flying", 0f);

        if (!cc.isGrounded){
            ySpeed += gravity * jumpScaler * Time.deltaTime;
            myAnimator.SetBool("Grounded", false);

            if (ySpeed < gravity)
            {
                ySpeed = gravity;
            }
        }
        else
        {
            myAnimator.SetTrigger("Landing");
            currentState = State.Grounded;
        }

        if (Input.GetButtonDown(flyToggle))
        {
            flaps = 2;
            flySpeed = baseFlySpeed;
            myAnimator.SetFloat("Flying", 1f);
            currentState = State.Flying;
        }

        velocity.y = ySpeed;

        cc.Move(velocity * Time.deltaTime);

    }

    public void Jump()
    {
        Debug.Log("Jump");
        myAnimator.SetBool("Grounded", false);

    }

    public void SetGrounded()
    {
        currentState = State.Grounded;

        rotateCoroutine = RotateMe();

        StartCoroutine(rotateCoroutine);
    }

    public IEnumerator RotateMe()
    {
        Debug.Log("Rotating");
        Quaternion fromAngle = transform.rotation;
        Quaternion toAngle = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));

        myAnimator.SetFloat("Flying", 0.9f);

        for (float i = 0f; i < 1f; i += Time.deltaTime / rotResetSpeed)
        {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, i);
            yield return null;
        }
    }

    void OnControllerColliderHit (ControllerColliderHit hit)
    {
        if (cc.isGrounded)
        {
            currentState = State.Grounded;
            
        }
        
        if(currentState == State.Flying)
        {
            currentState = State.Grounded;

            puppet.SetState(BehaviourPuppet.State.Unpinned);
            transform.rotation = Quaternion.identity;
        }
    }


}
