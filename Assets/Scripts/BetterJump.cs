using System.Diagnostics.Contracts;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


public class BetterJump : MonoBehaviour
{
    [SerializeField] float baseGravity;
    [SerializeField] float fallMultiplier;
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float maxFallSpeed;

    Rigidbody2D rb;

    bool jumpPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            //rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            rb.gravityScale = baseGravity * fallMultiplier;
            //rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else if (rb.linearVelocity.y > 0 && !jumpPressed)
        {
            //rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            rb.gravityScale = baseGravity * lowJumpMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
        else if (context.canceled)
        {
            jumpPressed = false;
        }
    }
}
