using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;

    public float GroundDrag;

    public float JumpForce;
    public float JumpCooldown;
    public float AirMultliplier;
    private bool isReadyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] private bool grounded;

    public Transform Orientation;
    
    public bool Freeze = false;
    public bool ActiveGrapple;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        //if (Freeze)
        //{
        //    rb.velocity = Vector3.zero;
        //    return;
        //}

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded && !ActiveGrapple)
            rb.drag = GroundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput() 
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && isReadyToJump && grounded)
        {
            isReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (ActiveGrapple) return;

        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection * MoveSpeed * 10, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection * MoveSpeed * 10 , ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (ActiveGrapple) return;

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVelocity.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * MoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isReadyToJump = true;
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        ActiveGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacmentY = endPoint.y - startPoint.y;
        Vector3 displacmentXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacmentXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacmentY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void ResetRestrictions()
    {
        ActiveGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<GrappleHook>().StopGrapple();
        }
    }

}
