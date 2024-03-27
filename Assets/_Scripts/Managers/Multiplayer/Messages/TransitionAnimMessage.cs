using Mirror;
namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct TransitionAnimMessage : NetworkMessage
    {
        public string TransitionName;
        public bool Play;
    }
}
