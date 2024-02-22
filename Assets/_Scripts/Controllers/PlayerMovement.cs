using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float speedIncreaseAmount = 2f;
    [SerializeField] private float speedDecreaseAmount = 2f; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal"); 

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();


        AdjustSpeed();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    //create invisible at player's feet. If the player is touching the ground, the player can jump.
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (horizontal != 0)
        {
            if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }


    private void AdjustSpeed()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            speed += speedIncreaseAmount;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            speed -= speedDecreaseAmount;
        }

        // Ensure speed doesn't go below zero
        speed = Mathf.Max(speed, 0.8f);
    }
}

