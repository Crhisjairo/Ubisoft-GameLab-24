using Mirror;
namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct MinigameStatusMessage : NetworkMessage
    {
        public bool StartMinigame;
        // Puedo mandar el timer value
    }
}
