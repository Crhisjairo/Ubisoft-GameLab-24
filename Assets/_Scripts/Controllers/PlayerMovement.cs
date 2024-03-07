using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    [RequireComponent(typeof(PlayerOneWayPlatform))]
    public class PlayerMovement : NetworkBehaviour
    {
        private Vector2 movementInput;
        private float speed = 8f;
        private float jumpingPower = 16f;
        private bool isFacingRight = true;
        private bool _jumped = false;

        private bool canDash = true;
        private bool isDashing = false;
        private float dashingPower = 24f;
        private float dashingTime = 0.05f;
        private float dashCooldown = 1.0f;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private TrailRenderer tr;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

        void Update()
        {
            if (!isLocalPlayer) return;
        
            if (isDashing)
            {
                return;
            }

            if (_jumped && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (!_jumped && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            Flip();
            
            AdjustSpeed();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (isDashing) return;
            
            
            movementInput = context.ReadValue<Vector2>();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            _jumped = context.action.triggered; // context.action.triggered;
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.action.triggered && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        private void FixedUpdate()
        {
            if (isDashing)
            {
                return;
            }
            rb.velocity = new Vector2(movementInput.x * speed, rb.velocity.y);
        }

        //create invisible at player's feet. If the player is touching the ground, the player can jump.
        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, 0.2f);
        }

        private void Flip()
        {
            if (movementInput.x != 0)
            {
                if ((isFacingRight && movementInput.x < 0f) || (!isFacingRight && movementInput.x > 0f))
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
            float[] speedStages = { 2.0f, 4.0f, 6.0f, 8.0f, 50.0f, 100.0f }; // Example speed stages

            // Check if not dashing before adjusting speed
            if (!isDashing)
            {
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

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            // Store the current horizontal velocity
            float originalHorizontalVelocity = rb.velocity.x;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            // Set the velocity for dashing
            rb.velocity = new Vector2(originalHorizontalVelocity * dashingPower, 0f);
            // Enable trail renderer
            tr.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            // Disable trail renderer
            tr.emitting = false;
            // Restore the original horizontal velocity
            rb.velocity = new Vector2(originalHorizontalVelocity, 0f);
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);

            canDash = true;
        }
    }
}
