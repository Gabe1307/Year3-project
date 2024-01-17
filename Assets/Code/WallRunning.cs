using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

  


    public float wallRunForce;
    public float maxWallRunTime;
    public float maxWallRunTimer;
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;
    // exiting wall
    private bool exitingWall;
    public float exitingWallTime;
    public float exitingWallTimer;

    [Header("Detection")]

    public float wallCheckDistance;
    public float minJumpHeight;

    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Refrences")]

    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;
    public PlayerCamera cam;

    [Header("Camera")]
    [SerializeField] private Camera Cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunfov;
    [SerializeField] private float wallRunFovTime;

    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;
    public float tilt { get; private set; }




    private void Start()
    {

        pm = GetComponent<PlayerMovement>();

        rb = GetComponent<Rigidbody>();
       
    }
    private void Update()
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if (pm.WallRunning)
            WallRunningMovement();
    }
    // shooting raycast out either side
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }
       // Shooting raycast down

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //WallRunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.WallRunning)
                StartWallRun();

            //wall jumping
            if (Input.GetKeyDown(jumpKey)) WallJump();

        }
        else if (exitingWall)
        {
            if (pm.WallRunning)
                StopWallRun();

            if (exitingWallTimer > 0)
                exitingWallTimer -= Time.deltaTime;

            if (exitingWallTimer <= 0)
                exitingWall = false;
        }

        else
        {
            if (pm.WallRunning)
            {
                StopWallRun();
            }
        }
    }
    private void StartWallRun()
    {
        pm.WallRunning = true;


        // apply cam orientation effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);
        
    }


    private void WallRunningMovement()
    {
        rb.useGravity = false;

       

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallforward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallforward).magnitude > (orientation.forward - -wallforward).magnitude)
            wallforward = -wallforward;

        //adding forward force
        rb.AddForce(wallforward * wallRunForce, ForceMode.Force);

        // pushing into the wall
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        rb.AddForce(-wallforward * 100, ForceMode.Force);

        
    }
    private void StopWallRun()
    {
        pm.WallRunning = false;
        rb.useGravity = true;

       cam.DoFov(80f);
       cam.DoTilt(0f);



    }
    private void WallJump()
    {
        exitingWall = true;
        exitingWallTimer = exitingWallTime;

        

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forcetoApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        // reset y velocity and Adding force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forcetoApply, ForceMode.Impulse);
      
    }
}
