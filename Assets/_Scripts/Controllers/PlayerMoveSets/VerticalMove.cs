using UnityEngine.InputSystem;
namespace _Scripts.Controllers.PlayerMoveSets
{
    public class VerticalMove : IPlayerMoveSetState
    {
        private PlayerMovement _player;
    
        public VerticalMove(PlayerMovement player)
        {
            _player = player;
        }
        
        public void Move(InputAction.CallbackContext context)
        {
            
        }

        public void Jump(InputAction.CallbackContext context)
        {
            
        }
        public void Dash(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
