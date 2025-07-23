using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    float horizontalMovement;
    public bool facingRight = true;
    public bool walled;

    [Header("Movement")]
    [SerializeField] float jumpForce;
    [SerializeField] float movementSpeed;

    [Header("Wall movement")]
    [SerializeField] float wallSlideSpeed;
    public bool wallSliding;

    public bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    [SerializeField] Vector2 wallJumpPower = new Vector2(5, 10);

    [Header("Ground check")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    [Header("Wall check")]
    [SerializeField] Transform wallCheckTransform;
    [SerializeField] Vector2 wallCheckSize;
    [SerializeField] LayerMask wallLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        Flip();
        WallSlide();
        ProcessWallJump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        walled = isWalled();

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * movementSpeed, rb.linearVelocity.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rb.AddForce(new Vector2(rb.linearVelocity.x, jumpForce));
        }

        //wall jump
        if(context.performed && wallJumpTimer > 0 && !isGrounded() && isWalled())
        {
            isWallJumping = true;
            rb.AddForce(new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y));
            wallJumpTimer = 0;

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }
    private bool isWalled()
    {
        return Physics2D.OverlapBox(wallCheckTransform.position, wallCheckSize, 0,  wallLayer);
    }

    void WallSlide()
    {
        if (!isGrounded() & isWalled() & horizontalMovement != 0)
        {
            wallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            wallSliding = false;
        }
    }

    void ProcessWallJump()
    {
        if (wallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    void CancelWallJump()
    {
        isWallJumping = false;
    }

    void Flip()
    {
        if (rb.linearVelocity.x < -0.1f)
        {
            facingRight = false;
        }
        else if (rb.linearVelocity.x > 0.1f)
        {
            facingRight = true;
        }

        if (facingRight)
        {
            wallCheckTransform.position = new Vector3(transform.position.x + 0.52f, transform.position.y, 0);
        }
        else if (!facingRight)
        {
            wallCheckTransform.position = new Vector3(transform.position.x + -0.52f, transform.position.y, 0);
        }
        /*if(facingRight && horizontalMovement < 0 || !facingRight && horizontalMovement > 0)
        {
            facingRight = !facingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);

        Gizmos.DrawWireCube(wallCheckTransform.position, wallCheckSize);
    }
}
