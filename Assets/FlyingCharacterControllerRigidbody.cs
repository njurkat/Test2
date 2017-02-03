using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;


public class FlyingCharacterControllerRigidbody : MonoBehaviour {

    public enum State
    {
        Grounded,
        Flying,
        Fallen,
        Airborne
    }

    [Header("State")]
    public State currentState;

    [Header("Components")]
    public Animator myAnimator;
    CharacterController cc;
    Transform cameraT;
    public BehaviourPuppet puppet { get; private set; }

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
    public float jumpHeight;
    public float landingDelay = 1.5f;
    public float ySpeed;
    float speedSmoothVelocity;
    float currentSpeed;
    float turnSmoothVelocity;
    public bool grounded;

    // Use this for initialization
    void Start () {
        myAnimator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        puppet = transform.parent.GetComponentInChildren<BehaviourPuppet>();
        cameraT = Camera.main.transform;
    }
	
	// Update is called once per frame
	void Update () {
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
        if (!grounded) return;

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

    }

    void Flying()
    {

    }

    void Fallen()
    {

    }

    void Airborne()
    {

    }
}
