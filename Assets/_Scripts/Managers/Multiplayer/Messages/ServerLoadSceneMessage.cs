using Mirror;

namespace _Scripts.Managers.Multiplayer.Messages
{
    public struct ServerLoadSceneMessage : NetworkMessage
    {
        public string SceneToLoad;
    }
}