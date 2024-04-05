using UnityEngine.InputSystem;
namespace _Scripts.Controllers.PlayerMoveSets
{
    public interface IPlayerMoveSetState
    {
        /// <summary>
        /// Move the player corresponding to the state.
        /// </summary>
        public void Move(InputAction.CallbackContext context);
        
        /// <summary>
        /// Apply jump to the player corresponding to the state.
        /// </summary>
        public void Jump(InputAction.CallbackContext context);
        
        /// <summary>
        /// Move the player with a dash corresponding to the state.
        /// </summary>
        /// <param name="context"></param>
        public void Dash(InputAction.CallbackContext context);

        public void FixedUpdateOnState();
    }
}
