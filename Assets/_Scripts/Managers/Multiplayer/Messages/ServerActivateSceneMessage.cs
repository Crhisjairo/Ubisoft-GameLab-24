using Mirror;

namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct ServerActivateSceneMessage : NetworkMessage
    {
        public string SceneName;
        public bool Activate;
    }
}