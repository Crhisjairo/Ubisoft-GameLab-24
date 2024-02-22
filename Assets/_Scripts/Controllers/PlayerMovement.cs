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
        // Define speed stages
        float[] speedStages = { 2.0f, 4.0f, 6.0f, 8.0f, 50.0f,100.0f }; // Example speed stages
                                                                
        float currentSpeed = speedStages[SpeedVariableCount.speedVariableCount - 1]; // Set current speed
        speed = currentSpeed; // Assign currentSpeed to speed

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // check if the next index of speedStages is not out of bounds
            if (SpeedVariableCount.speedVariableCount < speedStages.Length)
            {
                currentSpeed = speedStages[SpeedVariableCount.speedVariableCount];
                SpeedVariableCount.speedVariableCount++;
                speed = currentSpeed;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Decrease speed if not already at minimum
            if (SpeedVariableCount.speedVariableCount > 1)
            {
                currentSpeed = speedStages[SpeedVariableCount.speedVariableCount - 2];
                SpeedVariableCount.speedVariableCount--;
                speed = currentSpeed;
            }
        }

        // Ensure speed doesn't go below the minimum stage
        speed = Mathf.Max(speed, speedStages[0]);
    }


}

