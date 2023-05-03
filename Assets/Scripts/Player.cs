using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Vector3 movement;
    public Vector2 lookDirection;
    public Vector2 inputMovement;
    public State state;

    float movementForce = 2;

    Camera playerCamera;
    float yRotation;
    float xRotation;
    [SerializeField] Transform body;
    [SerializeField] Legs legs;


    float maxSpeed = 10f;
    Vector3 forceDirection = new Vector3();

    float mouseX;
    float mouseY;
    float mouseSensitivity = .5f;


    public enum State
    {
        Normal
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCamera = FindObjectOfType<Camera>();
        if (Follow.singleton != null)
        {
            Follow.singleton.SetTarget(this.transform);
        }
        state = State.Normal;
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        switch (state)
        {
            case State.Normal:
                HandleMovement();
                break;
        }

    }

    private void HandleInput()
    {
        mouseX = inputLook.x * mouseSensitivity * .1f;
        mouseY = inputLook.y * mouseSensitivity * .1f;
        FaceLookDirection();
    }
    Vector3 horizontalVelocity;
    void FixedUpdate()
    {
        HandleFixedInput();
        switch (state)
        {
            case State.Normal:
                HandleFixedMovement();
                break;
        }

        horizontalVelocity = rb.velocity;

        horizontalVelocity.y = 0;

        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

    }
    void FaceLookDirection()
    {
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89, 89);
        body.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        Follow.singleton.Rotate(body.transform);

    }
    private void HandleFixedInput()
    {
        forceDirection += movement.x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += movement.z * GetCameraForward(playerCamera) * movementForce;


    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forwardVec = playerCamera.transform.forward;
        forwardVec.y = 0;
        return forwardVec.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 rightVec = playerCamera.transform.right;
        rightVec.y = 0;
        return rightVec.normalized;
    }

    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;


        Vector3 horizontalVelocity1 = rb.velocity;

        horizontalVelocity1.y = 0;

        if (horizontalVelocity1.sqrMagnitude > maxSpeed * maxSpeed)
        {
            horizontalVelocity1 = horizontalVelocity1.normalized * maxSpeed;
        }
        if (movement.magnitude == 0)
        {
            legs.HandleStopping();
        }
        else
        {
            legs.HandleRunning(horizontalVelocity1, maxSpeed);
        }
    }
    private void HandleFixedMovement()
    {
        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        inputLook = value.Get<Vector2>();
    }
    Vector3 inputLook;
    void OnMousePosition(InputValue value)
    {
    }
}
