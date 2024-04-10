using Mirror;
namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct MinigameStatusMessage : NetworkMessage
    {
        public bool StartMinigame;
        public float TimerValue;
    }
}
