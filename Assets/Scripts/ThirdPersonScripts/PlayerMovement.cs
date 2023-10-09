using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed { get => moveSpeed; }

    private float moveSpeed;

    [Header("Movement")]
    public float GroundSpeed;
    public float AirSpeed;
    public float SwingSpeed;
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
    public bool isGrounded;
    public Transform Orientation;

    private bool hasStarted = false;
    private bool enableMovementOnNextTouch;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Rigidbody rb;

    public bool Freeze = false;
    public bool ActiveGrapple;
    public bool Swinging;

    public MovementState state;

    public enum MovementState
    {
        ground,
        grappling,
        swinging,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if (isGrounded && !ActiveGrapple)
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

        if (Input.GetKey(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void StateHandler()
    {
        if (isGrounded)
        {
            state = MovementState.ground;
            moveSpeed = GroundSpeed;

        }

        else if (ActiveGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = AirSpeed;
        }

        else if (Swinging)
        {
            state = MovementState.grappling;
            moveSpeed = SwingSpeed;
        }
    }

    private void MovePlayer()
    {
        if (ActiveGrapple || Swinging) return;

        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;
        rb.AddForce(moveDirection * moveSpeed * 10, ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (ActiveGrapple) return;

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
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



    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight, int grappleType)
    {
        ActiveGrapple = true;
        if (grappleType == 1)
        {
            velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
            velocityToSet = velocityToSet + (Vector3.up);
        }
        else if (grappleType == 2)
        {
            velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight * .25f);
        }

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
        if (!hasStarted)
        {
            hasStarted = true;
            return;
        }

        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            GetComponent<GrappleHook>().StopGrapple();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Pullable") || collision.gameObject.CompareTag("Pullable"))
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Grappleable") && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (ActiveGrapple)
                Debug.LogError("You're not Dead!");
            else
                Debug.LogError("You're Dead!");
        }

        ResetRestrictions();
    }

}
