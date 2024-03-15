using Mirror;

namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct SceneStatusMessage : NetworkMessage
    {
        public string SceneName;
        
        /// <summary>
        /// When the scene is loaded and ready to activate.
        /// </summary>
        public bool IsReady;
    }
}