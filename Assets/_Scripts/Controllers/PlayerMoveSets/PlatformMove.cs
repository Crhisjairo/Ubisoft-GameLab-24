using System.Collections;
using _Scripts.Shared;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _Scripts.Controllers.PlayerMoveSets
{
    public class PlatformMove : IPlayerMoveSetState
    {
        private PlayerMovement _player;
        
        public PlatformMove(PlayerMovement player)
        {
            _player = player;
            _player.rb.gravityScale = _player.defaultGravityScale;
            _player.rb.bodyType = RigidbodyType2D.Dynamic;
            _player.rb.isKinematic = false;
            _player.col.enabled = true;

            _player.speed = 18.0f;
            
            _player.IsDashing = false;
            _player.CanDash = true;
        }
        
        public void Move(InputAction.CallbackContext context)
        {
            if (_player.IsDashing) return;
            
            _player.MovementInput = context.ReadValue<Vector2>();

            _player.anim.SetBool(PlayerAnimations.isWalking.ToString(), 
                _player.MovementInput.x == 0);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            _player.Jumped = context.action.triggered; // context.action.triggered;
        }
        public void Dash(InputAction.CallbackContext context)
        {
            if (context.action.triggered && _player.CanDash)
            {
                _player.StartCoroutine(DashRoutine());
            }
        }

        public void FixedUpdateOnState()
        {
            if (_player.IsDashing) return;
            
            _player.rb.velocity = new Vector2(_player.MovementInput.x * _player.speed, _player.rb.velocity.y);
        }

        private IEnumerator DashRoutine()
        {
            _player.CanDash = false;
            _player.IsDashing = true;
            // Store the current horizontal velocity
            float originalHorizontalVelocity = _player.rb.velocity.x;
            float originalGravity = _player.rb.gravityScale;
            
            _player.rb.gravityScale = 0f;
            // Set the velocity for dashing
            _player.rb.velocity = new Vector2(originalHorizontalVelocity * _player.dashingPower, 0f);
            // Enable trail renderer
            _player.tr.emitting = true;
            yield return new WaitForSeconds(_player.dashingTime);
            // Disable trail renderer
            _player.tr.emitting = false;
            // Restore the original horizontal velocity
            _player.rb.velocity = new Vector2(originalHorizontalVelocity, 0f);
            _player.rb.gravityScale = originalGravity;
            _player.IsDashing = false;
            yield return new WaitForSeconds(_player.dashCooldown);

            _player.CanDash = true;
        }
    }
}
