using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer.Messages;
using _Scripts.Map;
using _Scripts.Shared;
using _Scripts.UI;
using _Scripts.UI.MainMenu;
using _Scripts.UI.PlayerUIs;
using Assets._Scripts.Shared;
using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
namespace _Scripts.Managers.Multiplayer
{
    public class MultiManager2 : NetworkManager
    {
        public override void Start()
        {
            base.Start();
            NetworkClient.RegisterHandler<PlayerComponentStatusMessage>(ClientReceivePlayerComponentStatus);
            
            NetworkServer.RegisterHandler<SceneStatusMessage>(ServerOnClientWithSceneLoaded);
            NetworkClient.RegisterHandler<MinigameStatusMessage>(OnReadyToStartMinigame);
            NetworkClient.RegisterHandler<TransitionAnimMessage>(ClientReceiveTransitionAnim);
        }

        #region  Server

        public float timeOnMinigame = 120f;
        public float timeOnBoss = 60f;
        
        public int PlayersOnLobbyCount { private set; get; }
        
        [SerializeField] private PlayerSlot _player1Slot;
        [SerializeField] private PlayerSlot _player2Slot;

        private int _playersWithSceneReady = 0;
        
        private bool _isTimerOn = true;
        private float _initialTime;
        private float _remainingTime;
        
        
        [SerializeField] private MinigameSceneNames[] scenesNamesToLoad;
        private Queue<MinigameSceneNames> _scenesSortedRan;

        [SerializeField] private CanvasGroup _canvasUIGroup;

        [Server]
        public void ServerStartGame()
        {
            StartCoroutine(SceneChangeRoutine());
        }

        private IEnumerator SceneChangeRoutine()
        {
            foreach(var sceneName in scenesNamesToLoad) //TODO: change to _scenesSortedRan
            {
                _playersWithSceneReady = 0;
            
                // FadeOut is played when server tells the clients to start the minigame in OnReadyToStartMinigame method.
                NetworkServer.SendToAll(new TransitionAnimMessage
                {
                    TransitionName = TransitionAnimations.FadeIn.ToString(),
                    Play = true
                });

                yield return new WaitForSeconds(1f); // Wait for the fade in animation to finish.
                ServerChangeScene(sceneName.ToString());
                
                // Wait until both players have the scene loaded.
                while(_playersWithSceneReady < 2)
                {
                    yield return null;
                }
                
                // Wait 5 seconds before starting the next minigame.
                yield return new WaitForSeconds(timeOnMinigame); // TODO: change for a timer.
                timeOnMinigame = timeOnBoss;
            }
            
            yield return null;
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            //Debug.Log(conn.connectionId + " is ready");
            PlayersOnLobbyCount++;
            
            if(PlayersOnLobbyCount == 1)
            {
                _player1Slot.SetIsReady(true);
            }
            else if(PlayersOnLobbyCount == 2)
            {
                _player2Slot.SetIsReady(true);
            }
            
            Debug.Log("Finished");
            
        }

        private MinigameSceneNames currentSceneName = MinigameSceneNames.Minigame_1;
        
        [Server]
        public void ServerOnClientWithSceneLoaded(NetworkConnectionToClient conn, SceneStatusMessage message)
        {
            // Contar hasta que los dos jugadores estên cargados. Una vez cargados, les aviso que activen el tiempo para jugar.
            //Debug.Log("Player " + conn.connectionId + " is ready to start minigame!");
            _playersWithSceneReady++;

            // Set Minigame settings like PlayerMoveSetStates, etc.
            // 0 = minigame_1, 1 = bossfight
            AssignPlayersNetEntitiesByScene(conn, _playersWithSceneReady);

            if (_playersWithSceneReady == 2)
            {
                NetworkServer.SendToAll(new MinigameStatusMessage
                {
                    StartMinigame = true,
                    TimerValue = timeOnMinigame,
                });

                Time.timeScale = 1;
                currentSceneName++;
            }
        }

        private void AssignPlayersNetEntitiesByScene(NetworkConnectionToClient conn, int player)
        {
           // Debug.Log("Assigning net entities to players on scene: " + currentSceneName + " for player " + player + "!");
            
            switch (currentSceneName)
            {
                case MinigameSceneNames.Minigame_1:
                    AssignNetEntitiesMinigame1(conn, player);
                    break;
                case MinigameSceneNames.Minigame_2:
                    AssignNetEntitiesBoss(conn, player);
                    break;
            }
        }

        private void AssignNetEntitiesMinigame1(NetworkConnectionToClient conn, int player)
        {
            conn.identity.GetComponent<PlayerMovement>().SetMoveSetState(PlayerMoveSetStates.VerticalMove);

            if (player == 1)
            {
                Hook hook = GameObject.FindWithTag("Player1Position").GetComponent<Hook>();
                hook.SetFollowTarget(conn);
            }
            else
            {
                Hook hook = GameObject.FindWithTag("Player2Position").GetComponent<Hook>();
                hook.SetFollowTarget(conn);
                
            }
            
        }

        
        private void AssignNetEntitiesBoss(NetworkConnectionToClient conn, int player)
        {
            conn.identity.GetComponent<PlayerMovement>().SetMoveSetState(PlayerMoveSetStates.PlatformMove);
            
            
        }
        
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            NetworkServer.SendToAll(new PlayerComponentStatusMessage()
            {
                componentName = "UI",
                isActive = true
            });
            Time.timeScale = 0;
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            conn.identity.GetComponent<PlayerInput>().enabled = false;
            
            //Debug.Log("PlayerOnLobbyCount: " + PlayersOnLobbyCount);

            // Player is notified on SetPlayerNumber method.
            if (numPlayers == 1)
            {
                SetPlayerNumber(conn.identity, PlayerIndex.Player1);
            }
            else if (numPlayers == 2)
            {
                SetPlayerNumber(conn.identity, PlayerIndex.Player2);
            }
            
            conn.Send(new PlayerComponentStatusMessage()
            {
                componentName = "PlayerInput",
                isActive = false
            });
        }
        
        private void SetPlayerNumber(NetworkIdentity identity, PlayerIndex playerIndex)
        {
            PlayerServerDataSync clientServerData  = identity.GetComponent<PlayerServerDataSync>();
            
            clientServerData.SetPlayerIndex(playerIndex);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            
            if(PlayersOnLobbyCount == 1)
            {
                _player1Slot.SetIsReady(false);
            }
            else if(PlayersOnLobbyCount == 2)
            {
                _player2Slot.SetIsReady(false);
            }

            PlayersOnLobbyCount--;
        }

        #endregion
        
        #region Client
        
        public CanvasGroupFade _canvasGroupFade;
        public TMP_InputField _ipField;
        public TextMeshProUGUI connectionErrorText;
        public MenuSection lobbySection;

        public override void Awake()
        {
            base.Awake();
            connectionErrorText.text = string.Empty;
        }

        public void ClientReceivePlayerComponentStatus(PlayerComponentStatusMessage message)
        {
            if (message.componentName == "PlayerInput")
            {
                NetworkClient.connection.identity.GetComponent<PlayerInput>().enabled = message.isActive;
            }
            
            if(message.componentName == "UI" && message.isActive)
            {
                _canvasUIGroup.alpha = 1;
            }
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            Time.timeScale = 0;

            NetworkClient.Send(new SceneStatusMessage
            {
                SceneName = SceneManager.GetActiveScene().name,
                IsReady = true
            });
        }

        public void OnReadyToStartMinigame(MinigameStatusMessage message)
        {
            //Debug.Log("Server says to start minigame and change timeScale!");
            if (message.StartMinigame)
            {
                Time.timeScale = 1;
            }
            
            HUDPlayersManager.Instance.SetInitialTimer(message.TimerValue);
            HUDPlayersManager.Instance.StartTutorialAnimation();
            
            // FadeOut is played when server tells the clients to start the minigame.
            ClientReceiveTransitionAnim(new TransitionAnimMessage
            {
                TransitionName = TransitionAnimations.FadeOut.ToString()
            });

            NetworkIdentity playerIdentity = NetworkClient.connection.identity;
            
            playerIdentity.GetComponent<PlayerInput>().enabled = true;
        }

        public void ClientReceiveTransitionAnim(TransitionAnimMessage message)
        {
            //Debug.Log("Receive " + message.TransitionName);
            
            if(message.TransitionName.Equals(TransitionAnimations.FadeIn.ToString()))
            {
                StartCoroutine(_canvasGroupFade.FadeIn());
            }
            else if(message.TransitionName.Equals(TransitionAnimations.FadeOut.ToString()))
            {
                StartCoroutine(_canvasGroupFade.FadeOut());
            }

        }

        public void StartClientBtn()
        {
            KcpTransport kcp = transport as KcpTransport;
            if(kcp == null)
            {
                Debug.LogError("KcpTransport is null");
                return;
            }

            //Debug.Log(_ipField.text);

            if (_ipField.text == string.Empty)
            {
                networkAddress = "localhost";
                kcp.Port = 7777;   
            }
            else
            {
                string[] ipPort = _ipField.text.Split(":");
                
                if(ipPort.Length != 2)
                {
                    connectionErrorText.text = "Please use ip:port";
                    return;
                }
                
                networkAddress = ipPort[0];
                if(ushort.TryParse(ipPort[1], out ushort port))
                    kcp.Port = port;
                else
                {
                    connectionErrorText.text = "Please use ip:port";
                    return;   
                }
            }
            
            StartClient();
        }

        
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            lobbySection.SetActive(true);
        }

        public override void OnClientError(TransportError error, string reason)
        {
            base.OnClientError(error, reason);
            connectionErrorText.text = 
                error == TransportError.Timeout 
                    ? TransportError.Timeout.ToString() 
                    : reason;

        }

        #endregion
    }
}
