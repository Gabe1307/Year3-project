using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;

    public float walkSpeed;

    public float sprintSpeed;
    public float wallRunSpeed;


    public float groundDrag;

    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump;
    //crouching
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    

    // keybind for jump
    public KeyCode jumpKey = KeyCode.Space;
    // keybind for sprinting
    public KeyCode sprintKey = KeyCode.LeftShift;
    // keybind for Crouching
    public KeyCode crouchKey = KeyCode.LeftControl;


    // Ground Check
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;


    public Transform orientation;

    // keyboard Inputs

    float horizontalInput;
    float verticalInput;

    Rigidbody rb;
    Vector3 moveDirection;

    public MovementState State;
    public enum MovementState
    {
        Walking, Sprinting,WallRunning, Air, Crouching
    }
    public bool WallRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }
    private void Update()
    {
        // Raycast Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);


        myInput();
        SpeedControl();
        StateHandler();

        // Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // jump control
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }
         

         // mode = sprinting
        private void StateHandler()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            State = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
        }
        // mode = Walking

        else if (grounded)
        {
            State = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
        // mode = Air
        else
        {
            State = MovementState.Air;

        }
        if (Input.GetKey(crouchKey))
        {
            State = MovementState.Crouching;
            moveSpeed = crouchSpeed;
        }
        if (WallRunning)
        {
            State = MovementState.WallRunning;
            moveSpeed = wallRunSpeed;
        }
    }

        private void MovePlayer()
        {
            // movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            // if on ground
            if (grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            else if (!grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);


        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // will limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }


        private void jump()
        {
            // resetting y velocity

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        }


        private void ResetJump()
        {
            readyToJump = true;
        }


    }



    
