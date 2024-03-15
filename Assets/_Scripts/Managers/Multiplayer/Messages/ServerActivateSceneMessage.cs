using Mirror;

namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct ServerActivateSceneMessage : NetworkMessage
    {
        string sceneName;
        bool activate;
    }
}