using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f * 2;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Transform cameraTransform;

    private float verticalVelocity;
    private Vector3 inputDirection = Vector3.zero;

    private float currentVel;
    private float smoothRotationTime = 0.12f;
    private bool isGrounded;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        inputDirection = new Vector3(x, 0, z).normalized;

        Debug.Log("Move Direction: " + inputDirection);

        // Handle rotation
        if (inputDirection.magnitude > 0.01f)
        {
            float rotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVel, smoothRotationTime);
        }

        // Handle jump & gravity
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        //check if the player is on the ground so he can jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //the equation for jumping
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = transform.forward;

        move *= inputDirection.magnitude * speed;
        move.y = verticalVelocity;

        Debug.Log(move);
        controller.Move(move * Time.deltaTime);
    }
}
