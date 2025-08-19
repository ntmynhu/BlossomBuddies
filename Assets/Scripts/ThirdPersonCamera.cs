using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float cameraDistance = 2f; // Distance from the player to the camera
    [SerializeField] private float rotationMin = -40f;
    [SerializeField] private float rotationMax = 90f;
    [SerializeField] private Camera thirdPersonCam;
    [SerializeField] private Transform followTarget;

    private float xRotation = 0f;
    private float YRotation = 0f;
    private float smoothTime = 0.12f;

    private Vector3 currentVel;
    private Vector3 targetRotation;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        YRotation += mouseX;

        //clamp the rotation so we cant Over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, rotationMin, rotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(xRotation, YRotation), ref currentVel, smoothTime);

        //applying both rotations
        thirdPersonCam.transform.eulerAngles = targetRotation;
        thirdPersonCam.transform.position = followTarget.position - thirdPersonCam.transform.forward * cameraDistance;
    }
}
