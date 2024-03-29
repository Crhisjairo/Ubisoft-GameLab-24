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
        }
        
        public void Move(InputAction.CallbackContext context)
        {
            if (_player.IsDashing) return;
            
            _player.MovementInput = context.ReadValue<Vector2>();
            
            if(_player.MovementInput.x == 0)
            {
                _player.anim.SetBool(PlayerAnimations.isWalking.ToString(), true);
            }
            else
            {
                _player.anim.SetBool(PlayerAnimations.isWalking.ToString(), false);
            }
        }

        public void Jump(InputAction.CallbackContext context)
        {
            _player.Jumped = context.action.triggered; // context.action.triggered;
        }
        public void Dash(InputAction.CallbackContext context)
        {
            if (context.action.triggered && _player.CanDash)
            {
                _player.StartCoroutine(_player.DashRoutine());
            }
        }
    }
}
