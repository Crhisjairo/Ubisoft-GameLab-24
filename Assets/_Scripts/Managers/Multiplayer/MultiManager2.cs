using System.Collections;
using _Scripts.Managers.Multiplayer.Messages;
using _Scripts.UI.PlayerUIs;
using Assets._Scripts.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace _Scripts.Managers.Multiplayer
{
    public class MultiManager2 : NetworkManager
    {
        public override void Start()
        {
            base.Start();
            NetworkClient.RegisterHandler<PlayerComponentStatusMessage>(ClientReceivePlayerComponentStatus);
        }

        #region  Server
        public int PlayerReadyCount { private set; get; }
        
        [SerializeField] private PlayerSlot _player1Slot;
        [SerializeField] private PlayerSlot _player2Slot;

        public void ServerStartGame()
        {
            ServerChangeScene(MinigameSceneNames.Minigame_1.ToString());
        }
        
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            Debug.Log(conn.connectionId + " is ready");
            PlayerReadyCount++;
            
            if(PlayerReadyCount == 1)
            {
                _player1Slot.SetIsReady(true);
            }
            else if(PlayerReadyCount == 2)
            {
                _player2Slot.SetIsReady(true);
            }
            
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            conn.identity.GetComponent<PlayerInput>().enabled = false;
            
            conn.Send(new PlayerComponentStatusMessage()
            {
                componentName = "PlayerInput",
                isActive = false
            });
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            
            if(PlayerReadyCount == 1)
            {
                _player1Slot.SetIsReady(false);
            }
            else if(PlayerReadyCount == 2)
            {
                _player2Slot.SetIsReady(false);
            }

            PlayerReadyCount--;
        }

        #endregion
        
        #region Client

        public void ClientReceivePlayerComponentStatus(PlayerComponentStatusMessage message)
        {
            if (message.componentName == "PlayerInput")
            {
                NetworkClient.connection.identity.GetComponent<PlayerInput>().enabled = message.isActive;
            }

        }
        
        
        #endregion
    }
}
