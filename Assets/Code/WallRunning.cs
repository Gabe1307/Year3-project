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

    [Header("Camera Tilt")]
    public float tilt;
    public float camTilt;
    public float camTiltTime;

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

        // camera tilt
        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
    }
    private void StopWallRun()
    {
        pm.WallRunning = false;
        rb.useGravity = true;
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
