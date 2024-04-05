using System;
using System.Collections;
using _Scripts.Controllers.PlayerMoveSets;
using _Scripts.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    [RequireComponent(typeof(PlayerOneWayPlatform))]
    public class PlayerMovement : NetworkBehaviour
    {
        public Vector2 MovementInput { set; get; }
        public float speed { private set; get; } = 8f ;
        public float jumpingPower { private set; get; } = 16f;
        private bool isFacingRight = true;
        public bool Jumped { set; get; } = false;

        public bool CanDash { set; get; } = true;
        public bool IsDashing { set; get; }
        public float dashingPower { private set; get; } = 24f;
        public float dashingTime { private set; get; } = 0.05f;
        public float dashCooldown { private set; get; } = 1.0f;
        
        public float defaultGravityScale = 4.0f;
        
        public PlayerMoveSetStates defaultMoveSetState = PlayerMoveSetStates.PlatformMove;
        
        /// <summary>
        /// This variable is used to sync the move set state between the server and the client.
        /// </summary>
        [SyncVar(hook = nameof(OnUpdateMoveSetOnServer))]
        private PlayerMoveSetStates _currentMoveSetState;
        
        /// <summary>
        /// This interface is used to control the player's movement based on the current move set state.
        /// </summary>
        private IPlayerMoveSetState _currentMoveSetInterface;

        public Rigidbody2D rb;
        public Collider2D col;
        public Animator anim;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        public TrailRenderer tr;

        private void Awake()
        {
            SetMoveSetState(defaultMoveSetState);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

        void Update()
        {
            if (!isLocalPlayer) return;

            if (IsDashing)
            {
                return;
            }

            if (Jumped && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (!Jumped && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            Flip();
            
            AdjustSpeed();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _currentMoveSetInterface.Move(context);
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            _currentMoveSetInterface.Jump(context);
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            _currentMoveSetInterface.Dash(context);
        }

        private void FixedUpdate()
        {
            _currentMoveSetInterface.FixedUpdateOnState();
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
            if (MovementInput.x != 0)
            {
                if ((isFacingRight && MovementInput.x < 0f) || (!isFacingRight && MovementInput.x > 0f))
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
            if (!IsDashing)
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

        public void SetMoveSetState(PlayerMoveSetStates newState)
        {
            switch (newState)
            {
                case PlayerMoveSetStates.VerticalMove:
                    _currentMoveSetInterface = new VerticalMove(this);
                    _currentMoveSetState = PlayerMoveSetStates.VerticalMove;
                    break;
                case PlayerMoveSetStates.PlatformMove:
                    _currentMoveSetInterface = new PlatformMove(this);
                    _currentMoveSetState = PlayerMoveSetStates.PlatformMove;
                    break;
                default:
                    Debug.Log("Invalid state. Setting to default \"" + nameof(PlatformMove) + "\"  state.");
                    _currentMoveSetInterface = new PlatformMove(this);
                    _currentMoveSetState = PlayerMoveSetStates.PlatformMove;
                    break;
            }
        }

        /// <summary>
        /// When the move set is updated on the server, update the client's move set.
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        private void OnUpdateMoveSetOnServer(PlayerMoveSetStates oldState, PlayerMoveSetStates newState)
        {
            SetMoveSetState(newState);
        }
    }
}
