using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FlyingCharacterController : MonoBehaviour {

    public enum State
    {
        Flying,
        Grounded,
        Fallen,
        Airborne
    }

    public Transform target;

    [Header("Current State")]
    public State currentState;

    [Header("References")]
    public Camera myCamera;
    public BehaviourPuppet puppet { get; private set; }

    private Animator myAnimator;
    private CharacterController cc;
    

    [Header("Controls")]
    public string horizontalAxis;
    public string verticalAxis;
    public string jumpButton;
    public string flyButton;

    [Header("Speeds")]
    public float groundSpeed;
    public float airSpeed;
    public float turnSpeed = 15f;
    public float jumpHeight = 15f;
    public float jumpScaler = 5f;
    public float fallRate = -1.5f;
    public float ySpeed;
    public float gravity = -9.8f;

    [Header("Bools")]
    public bool jumping;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        currentState = State.Grounded;
        puppet = transform.parent.GetComponentInChildren<BehaviourPuppet>();
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
        Vector3 input = new Vector3(Input.GetAxisRaw(horizontalAxis), 0, Input.GetAxisRaw(verticalAxis));
        Vector3 inputDir = input.normalized;

        transform.eulerAngles = Vector3.up * Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
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

    public void Jump()
    {

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }


}
