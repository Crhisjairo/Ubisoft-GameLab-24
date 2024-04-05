using System.Collections;
using _Scripts.Shared;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _Scripts.Controllers.PlayerMoveSets
{
    public class VerticalMove : IPlayerMoveSetState
    {
        private PlayerMovement _player;
        
        public VerticalMove(PlayerMovement player)
        {
            _player = player;
            _player.rb.gravityScale = 0f;
            _player.col.enabled = false;
        }
        
        public void Move(InputAction.CallbackContext context)
        {
            if (_player.IsDashing) return;
            
            var movement = context.ReadValue<Vector2>();
            movement.x = 0;
            
            _player.MovementInput = movement;

            _player.anim.SetBool(PlayerAnimations.isWalking.ToString(), 
                _player.MovementInput.y == 0);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            
        }
        
        public void Dash(InputAction.CallbackContext context)
        {
            if (context.action.triggered && _player.CanDash)
            {
                _player.StartCoroutine(VerticalDashRoutine());
            }
        }

        public void FixedUpdateOnState()
        {
            if (_player.IsDashing) return;

            _player.rb.velocity = new Vector2(_player.rb.velocity.x, _player.MovementInput.y * _player.speed);
        }
        
        private IEnumerator VerticalDashRoutine()
        {
            _player.CanDash = false;
            _player.IsDashing = true;
            // Store the current horizontal velocity
            float originalVerticalVelocity = _player.rb.velocity.y;
            float originalGravity = _player.rb.gravityScale;
            
            _player.rb.gravityScale = 0f;
            // Set the velocity for dashing
            _player.rb.velocity = new Vector2(0f, originalVerticalVelocity * _player.dashingPower);
            // Enable trail renderer
            _player.tr.emitting = true;
            yield return new WaitForSeconds(_player.dashingTime);
            // Disable trail renderer
            _player.tr.emitting = false;
            // Restore the original horizontal velocity
            _player.rb.velocity = new Vector2(0f, originalVerticalVelocity);
            _player.rb.gravityScale = originalGravity;
            _player.IsDashing = false;
            yield return new WaitForSeconds(_player.dashCooldown);

            _player.CanDash = true;
        }
    }
}
