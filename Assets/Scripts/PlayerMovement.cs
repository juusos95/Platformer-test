using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float horizontalMovement;
    bool facingRight = true;
    public bool grounded;
    public bool walled;

    [SerializeField] PhysicsMaterial2D normalPhysics;
    [SerializeField] PhysicsMaterial2D frictionlessPhysics;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float jumpForce;
    [SerializeField] float maxFallSpeed;

    [Header("Ground check")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] Vector2 groundCheckSize;
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

    // Update is called once per frame
    void Update()
    {
        grounded = isGrounded();
        walled = isWalled();

        Flip();
    }
    private void FixedUpdate()
    {
        rb.AddForce(new Vector2(horizontalMovement * movementSpeed, 0));
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -maxMoveSpeed, maxMoveSpeed), Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded() && context.performed)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    void Flip()
    {
        if (facingRight && horizontalMovement < 0 || !facingRight && horizontalMovement > 0)
        {
            facingRight = !facingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    void ChangePhysicsMaterial()
    {
        if (!isGrounded() && isWalled() && facingRight && rb.linearVelocity.x > 0 || !isGrounded() && isWalled() && !facingRight && rb.linearVelocity.x < 0)
        {
            if (rb.sharedMaterial != frictionlessPhysics)
            {
                rb.sharedMaterial = frictionlessPhysics;
            }
        }
        else if (rb.sharedMaterial != normalPhysics)
        {
            rb.sharedMaterial = normalPhysics;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckTransform.position, groundCheckSize, 0, groundLayer);
    }

    private bool isWalled()
    {
        return Physics2D.OverlapBox(wallCheckTransform.position, wallCheckSize, 0, wallLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);

        Gizmos.DrawWireCube(wallCheckTransform.position, wallCheckSize);
    }
}
