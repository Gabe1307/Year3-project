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


    [Header("Input")]

    private float horizontalInput;
    private float verticalInput;

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
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.WallRunning)
                StartWallRun();
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
    }
    private void StopWallRun()
    {
        pm.WallRunning = false;
        rb.useGravity = true;
    }
}
