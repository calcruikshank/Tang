using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Queue<BufferInput> bufferQueue = new Queue<BufferInput>();

    Rigidbody rb;
    Vector3 movement;
    public Vector2 lookDirection;
    public Vector2 inputMovement;
    public State state;

    float movementForce = 40;

    Camera playerCamera;
    float yRotation;
    float xRotation;
    [SerializeField] Transform body;
    [SerializeField] Legs legs;
    [SerializeField] Vector3 BottomLeftSwordPosition;
    [SerializeField] Quaternion BottomLeftSwordRotation;
    [SerializeField] Vector3 BottomRightSwordPosition;
    [SerializeField] Quaternion BottomRighSwordRotation;
    [SerializeField] Vector3 TopRightSwordPosition;
    [SerializeField] Quaternion TopRightSwordRotation;
    [SerializeField] Vector3 TopLeftSwordPosition;
    [SerializeField] Quaternion TopLeftSwordRotation;

    [SerializeField] Transform visualForMousePosition;
    [SerializeField] Transform halfwayPointVisual;

    Vector3 swordStartAttackPosition = new Vector3();

    float bufferTimerThreshold = .2f;
    Vector3 currentMousePositionInsideBox = new Vector3();
    float swordX;
    float swordY;
    float swordPositionZ;
    [SerializeField] Transform swingTarget;
    [SerializeField] Transform actualSwingTarget;
    [SerializeField] Transform swordRotationWhileSwinging;
    [SerializeField] Transform swordPathTransform;
    [SerializeField] Transform initialLeftClickPositionTransform;
    [SerializeField] Transform finalLeftClickPositionTransform;

    float bottomLimitPosition = 90;
    float topLimitPosition = -54;


    Vector3 directionOfHalfwayAttack;

    Vector3 horizontalVelocity;

    Vector3 inputLook;
    float maxSpeed = 10;
    Vector3 forceDirection = new Vector3();

    float mouseX;
    float mouseY;
    float mouseSensitivity = .5f;

    [SerializeField] Transform footParent;
    [SerializeField] Transform handParent;
    bool leftClickPressed = false;


    Vector3 offsetAfterLeftClick;
    public enum State
    {
        Normal,
        Attacking
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
        visualForMousePosition.localPosition = new Vector3(0, currentMousePositionInsideBox.y, currentMousePositionInsideBox.z);
        swordRotationWhileSwinging.LookAt(swingTarget, Vector3.up);
        swordRotationWhileSwinging.transform.position = handParent.transform.position;
        HandleBufferInput();
        HandleInput();
        switch (state)
        {
            case State.Normal:
                HandleMovement();
                HandleRegularSwordPosition();
                break;
        }
        switch (state)
        {
            case State.Attacking:
                HandleMovement();
                HandleAttackingSwordPosition();
                break;
        }

    }
    void FixedUpdate()
    {
        HandleFixedInput();
        switch (state)
        {
            case State.Normal:
                HandleFixedMovement();
                break;
            case State.Attacking:
                HandleFixedMovement();
                break;
        }

    }

    private void HandleInput()
    {
        mouseX = inputLook.x * mouseSensitivity * .1f;
        mouseY = inputLook.y * mouseSensitivity * .1f;
        FaceLookDirection();
    }
    void FaceLookDirection()
    {
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20, 20);
        body.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        Follow.singleton.Rotate(new Vector3(body.transform.localEulerAngles.x, transform.localEulerAngles.y, body.transform.localEulerAngles.z));
        //footParent.localEulerAngles = movement.normalized;

        if (rb.velocity.magnitude > .1f)
        {
            footParent.forward = new Vector3(rb.velocity.normalized.x, 0, rb.velocity.normalized.z);
        }
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



        horizontalVelocity = rb.velocity;

        horizontalVelocity.y = 0;

        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    Vector3 initialLeftClickPosition = new Vector3();
    void OnFire()
    {
        initialLeftClickPosition = new Vector3(visualForMousePosition.position.x, visualForMousePosition.position.y, visualForMousePosition.position.z);
        initialLeftClickPositionTransform.position = initialLeftClickPosition;
        leftClickPressed = true;
    }

    Vector3 finalLeftClickPosition = new Vector3();
    void OnFireUp()
    {
        finalLeftClickPosition = new Vector3(visualForMousePosition.position.x, visualForMousePosition.position.y, visualForMousePosition.position.z);
        finalLeftClickPositionTransform.position = new Vector3( finalLeftClickPosition.x, finalLeftClickPosition.y, finalLeftClickPosition.z);
        finalLeftClickPositionTransform.localPosition = new Vector3(finalLeftClickPositionTransform.localPosition.x - 200, finalLeftClickPositionTransform.localPosition.y, finalLeftClickPositionTransform.localPosition.z);
        leftClickPressed = false;
        BufferInput attackBuffer = new BufferInput(TangData.InputActionType.LeftClick, currentMousePositionInsideBox.normalized, Time.time);
        bufferQueue.Enqueue(attackBuffer);
    }

    void OnLook(InputValue value)
    {
        inputLook = value.Get<Vector2>();
    }

    Vector3 midpointOfAttack;
    public void HandleRegularSwordPosition()
    {
        float swordXCheck = swordY;
        swordX += inputLook.x;
        swordY += -inputLook.y;
        if (swordX > TopRightSwordPosition.y)
        {
            swordX = TopRightSwordPosition.y;
        }
        if (swordX < TopLeftSwordPosition.y)
        {
            swordX = TopLeftSwordPosition.y;
        }
        if (swordY > bottomLimitPosition)
        {
            swordY = bottomLimitPosition;
        }
        if (swordY < topLimitPosition)
        {
            swordY = topLimitPosition;
        }
        swordPositionZ = (MathF.Abs(swordX) - 240);

        currentMousePositionInsideBox = new Vector3(swordPositionZ, swordX, swordY);



        float xDistanceFromLeft = TopLeftSwordPosition.y - swordX;

        float zeroToOneX = xDistanceFromLeft / MathF.Abs(TopLeftSwordPosition.y * 2);
        zeroToOneX = Mathf.Abs(zeroToOneX);

        float yDistanceFromTop = bottomLimitPosition - swordY;

        float zeroToOneY = yDistanceFromTop / MathF.Abs(bottomLimitPosition);
        zeroToOneY = Mathf.Abs(zeroToOneY);
        //The closer swordx is to zero i want to rotate the sword up to 180

        Quaternion xRot = Quaternion.Lerp(TopLeftSwordRotation, TopRightSwordRotation, zeroToOneX);
        Quaternion yRot = Quaternion.Lerp(BottomRighSwordRotation, TopRightSwordRotation, zeroToOneY);


        if (!leftClickPressed)
        {

            handParent.transform.localRotation = Quaternion.RotateTowards(handParent.transform.localRotation, xRot, 500 * Time.deltaTime);
            handParent.transform.localPosition = Vector3.MoveTowards(handParent.transform.localPosition, currentMousePositionInsideBox, 50 * Time.deltaTime * Vector3.Distance(currentMousePositionInsideBox, handParent.transform.localPosition));
        }

        if (leftClickPressed)
        {
            offsetAfterLeftClick = -initialLeftClickPositionTransform.localPosition;

            directionOfAttack = Vector3.zero - (new Vector3(visualForMousePosition.localPosition.x, visualForMousePosition.localPosition.y, visualForMousePosition.localPosition.z) + offsetAfterLeftClick);



            swordPathTransform.transform.position = initialLeftClickPositionTransform.position;
            swordPathTransform.transform.LookAt(visualForMousePosition, Vector3.up);


            midpointOfAttack = Vector3.Lerp(initialLeftClickPositionTransform.position, visualForMousePosition.position, 0.5f);

            halfwayPointVisual.transform.position = initialLeftClickPositionTransform.position;
            halfwayPointVisual.transform.LookAt(actualSwingTarget);



            /*if (directionOfAttack.y >= 0)
            {
                if (directionOfAttack.z < 0)
                {
                    //then its good
                    handParent.transform.forward = swordPathTransform.up;
                }
                else
                {
                    swordPathTransform.transform.forward = swordPathTransform.transform.up;


                    handParent.transform.forward = new Vector3(swordPathTransform.up.x + 180, swordPathTransform.up.y, swordPathTransform.up.z + 180);
                    //then i want to flip
                }
            }
            else if (directionOfAttack.y < 0)
            {
            }*/

            //handParent.transform.rotation = Quaternion.RotateTowards(handParent.transform.rotation, halfwayPointVisual.rotation, 500 * Time.deltaTime);
            handParent.transform.rotation = Quaternion.RotateTowards(handParent.transform.rotation, swordPathTransform.rotation, 5000 * Time.deltaTime);
            //handParent.transform.forward = Vector3.MoveTowards(handParent.transform.forward, new Vector3(swordPathTransform.transform.forward.x, swordPathTransform.transform.forward.y, swordPathTransform.transform.forward.z), 250 * Time.deltaTime);
            


            //handParent.up = Vector3.MoveTowards(handParent.up, - swordRotationWhileSwinging.forward, 10 * Time.deltaTime);
        }
        /*if (MathF.Abs(swordX) > 80 || swordY < 30)
        {
            handParent.transform.localRotation = Quaternion.RotateTowards(handParent.transform.localRotation, new Quaternion(xRot.x, yRot.y, yRot.z, xRot.w), 500 * Time.deltaTime);
        }
        else
        {
        handParent.transform.localRotation = Quaternion.RotateTowards(handParent.transform.localRotation, xRot, 500 * Time.deltaTime);
        }*/



    }


    bool isFollowThrough = false;

    float attackTimer = 0;
    float attackTimerThreshhold = .2f;

    float xRotWhileAttacking = 0;

    private void HandleAttackingSwordPosition()
    {
        attackTimer += Time.deltaTime;

        if (directionOfAttack.y < 0)
        {
            handParent.RotateAround(handParent.transform.position, handParent.up, 1000 * Time.deltaTime);
        }
        else
        {
            handParent.RotateAround(handParent.transform.position, -handParent.up, 1000 * Time.deltaTime);
        }

        float distanceFromHandParentReachingTarget = Vector3.Distance(handParent.forward, -swordPathTransform.forward);

        Debug.Log(distanceFromHandParentReachingTarget + " " + Time.deltaTime);

        float zeroToOneTimer = attackTimer / attackTimerThreshhold;

        //Vector3 swordPositionWhileAttacking = Vector3.Lerp(handParent.transform.localPosition, finalLeftClickPositionTransform.localPosition, zeroToOneX);
        handParent.transform.localPosition = Vector3.Lerp(handParent.transform.localPosition, finalLeftClickPositionTransform.localPosition, zeroToOneTimer);

        //handParent.transform.localPosition = Vector3.MoveTowards(handParent.transform.localPosition, finalLeftClickPositionTransform.localPosition, 5 * Time.deltaTime * Vector3.Distance(currentMousePositionInsideBox, finalLeftClickPositionTransform.localPosition));
        xRotWhileAttacking += 1;
        if (attackTimer > attackTimerThreshhold)
        {
            ChangeStateToNormal();
        }
        //handParent.rotation = Quaternion.RotateTowards(handParent.rotation, halfwayPointVisual.rotation, 100 * Time.deltaTime);
        return;
        if (!isFollowThrough)
        {
            //handParent.up = Vector3.MoveTowards(handParent.forward, swordPathTransform.up, 100 * Time.deltaTime);

            if (Vector3.Distance(handParent.up, swordRotationWhileSwinging.up) < .1f)
            {
                //isFollowThrough = true;
            }
        }
        else
        {
            handParent.up = Vector3.MoveTowards(handParent.up, transform.up, 100 * Time.deltaTime);
            handParent.transform.localPosition = Vector3.MoveTowards(handParent.transform.position, new Vector3(currentMousePositionInsideBox.x - 100, currentMousePositionInsideBox.y, currentMousePositionInsideBox.z), 100 * Time.deltaTime);
        }
    }

    private void ChangeStateToNormal()
    {
        attackTimer = 0;
        isFollowThrough = false;
        state = State.Normal;
    }

    void HandleBufferInput()
    {
        if (bufferQueue.Count > 0)
        {
            BufferInput currentBufferedInput = (BufferInput)bufferQueue.Peek();

            if (Time.time - currentBufferedInput.timeOfInput < bufferTimerThreshold)
            {
                if (currentBufferedInput.actionType == TangData.InputActionType.Jump)
                {
                    /*if (grounded)
                    {
                        Jump();
                        inputQueue.Dequeue();
                    }
                    if (!grounded && !groundHasNotBeenLeftAfterJumping && currentNumOfExtraJumps < numOfExtraJumps)
                    {
                        kangarooJacked.transform.localScale = new Vector3(.187f * MathF.Sign(lastMoveDir.x), kangarooJacked.transform.localScale.y, kangarooJacked.transform.localScale.z);
                        currentNumOfExtraJumps++;
                        rb.velocity = new Vector2(movement.x * maxSpeed, 0);
                        Jump();
                        inputQueue.Dequeue();
                    }*/
                }
                /*if (currentBufferedInput.actionType == KangarooJackedData.InputActionType.DASH)
                {
                    if (state == State.Normal)
                    {
                        if (grounded)
                        {
                            Dash(new Vector2(currentBufferedInput.directionOfAction.x, 0));
                        }
                        else
                        {
                            Dash(currentBufferedInput.directionOfAction);
                        }
                        inputQueue.Dequeue();
                    }
                }*/
                if (currentBufferedInput.actionType == TangData.InputActionType.LeftClick)
                {
                    if (state == State.Normal)
                    {
                        if (currentBufferedInput.directionOfAction != Vector3.zero)
                        {
                            ChangeStateToAttack(new Vector3(currentBufferedInput.directionOfAction.x, currentBufferedInput.directionOfAction.y));
                            bufferQueue.Dequeue();
                        }
                    }
                }
            }
            if (Time.time - currentBufferedInput.timeOfInput >= bufferTimerThreshold)
            {
                bufferQueue.Dequeue();
            }
        }
    }


    Vector3 directionOfAttack;
    private void ChangeStateToAttack(Vector3 vector3Sent)
    {
        isFollowThrough = false;
        state = State.Attacking;
        xRotWhileAttacking = handParent.localRotation.x;
    }

}
