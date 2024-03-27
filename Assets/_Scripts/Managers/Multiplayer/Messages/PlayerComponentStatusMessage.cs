using Mirror;
namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct PlayerComponentStatusMessage : NetworkMessage
    {
        public string componentName;
        public bool isActive;
    }
}
