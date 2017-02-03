using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public enum CameraMode
    {
        Grounded,
        Flying
    }

    public CameraMode cameraMode;

    public float distanceAway;
    public float distanceUp;
    public float smooth;

    public float pitch;
    public Vector2 pitchMinMax = new Vector2(-10f, 85f);
    public float yaw;
    public float lookSensitivity = 5f;

    public float rotationSmoothTime;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public Transform target;
    private Vector3 targetPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void LateUpdate()
    {

        yaw += Input.GetAxis("RHorizontal") * lookSensitivity;
        pitch -= Input.GetAxis("RVertical") * lookSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distanceAway;

        switch (cameraMode)
        {
            case CameraMode.Grounded:
                Grounded();
                break;

            case CameraMode.Flying:
                Flying();
                break;
        }
    }

    void Grounded()
    {
        
    }

    void Flying()
    {

    }
}
