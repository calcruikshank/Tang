using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Vector3 lastLookedPosition;
    Vector3 movement;
    public Vector2 lookDirection;
    Vector3 currentMovement;
    public Vector2 inputMovement;
    public State state;
    public enum State
    {
        Normal
    }
    // Start is called before the first frame update
    void Start()
    {
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
        switch (state)
        {
            case State.Normal:
                HandleMovement();
                break;
        }

    }
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                HandleFixedMovement();
                break;
        }

    }
    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;
    }
    private void HandleFixedMovement()
    {
        rb.AddForce(movement * 10);
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }
}
