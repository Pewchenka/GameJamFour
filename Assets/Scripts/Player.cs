using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 0f;
    public float dashSpeed = 50f;
    public float dashTime = 0.4f;
    public float dashBuffer = 0.2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float zRotation = 0f;
    public LayerMask groundlayer;


    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isDashing = false;
    private bool canDash;


    public int extraJumpsValue = 1;
    private int extraJumps;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;
        canDash = true;
    }


    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
            canDash = true;
        }

        if (moveInput < 0)
            spriteRenderer.flipX = true;
        else if (moveInput > 0)
            spriteRenderer.flipX = false;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }

        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundlayer);
    }
}
