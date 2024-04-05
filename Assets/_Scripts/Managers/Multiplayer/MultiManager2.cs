using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer.Messages;
using _Scripts.Map;
using _Scripts.Shared;
using _Scripts.UI.MainMenu;
using _Scripts.UI.PlayerUIs;
using Assets._Scripts.Shared;
using kcp2k;
using Mirror;
using TMPro;
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
            
            NetworkServer.RegisterHandler<SceneStatusMessage>(ServerOnClientWithSceneLoaded);
            NetworkClient.RegisterHandler<MinigameStatusMessage>(OnReadyToStartMinigame);
            NetworkClient.RegisterHandler<TransitionAnimMessage>(ClientReceiveTransitionAnim);
        }

        #region  Server

        public float timeOnMinigameDebug = 60f;
        
        public int PlayerReadyCount { private set; get; }
        
        [SerializeField] private PlayerSlot _player1Slot;
        [SerializeField] private PlayerSlot _player2Slot;

        private int _playersWithSceneReady = 0;
        
        private bool _isTimerOn = true;
        private float _initialTime;
        private float _remainingTime;
        
        
        [SerializeField] private MinigameSceneNames[] scenesNamesToLoad;
        private Queue<MinigameSceneNames> _scenesSortedRan;

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
                yield return new WaitForSeconds(timeOnMinigameDebug); // TODO: change for a timer.
            }
            
            yield return null;
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

        private MinigameSceneNames currentSceneName = MinigameSceneNames.Minigame_1;
        
        [Server]
        public void ServerOnClientWithSceneLoaded(NetworkConnectionToClient conn, SceneStatusMessage message)
        {
            // Contar hasta que los dos jugadores estên cargados. Una vez cargados, les aviso que activen el tiempo para jugar.
            Debug.Log("Player " + conn.connectionId + " is ready to start minigame!");
            _playersWithSceneReady++;

            // Set Minigame settings like PlayerMoveSetStates, etc.
            // 0 = minigame_1, 1 = bossfight
            AssignPlayersNetEntitiesByScene(conn, _playersWithSceneReady);

            if (_playersWithSceneReady == 2)
            {
                NetworkServer.SendToAll(new MinigameStatusMessage
                {
                    StartMinigame = true
                });

                Time.timeScale = 1;
                currentSceneName++;
            }
        }

        private void AssignPlayersNetEntitiesByScene(NetworkConnectionToClient conn, int player)
        {
            switch (currentSceneName)
            {
                case MinigameSceneNames.Minigame_1:
                    AssignNetEntitiesMinigame1(conn, player);
                    break;
                case MinigameSceneNames.Boss:
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
            Time.timeScale = 0;
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

        public override void Update()
        {
            base.Update();
            // UpdateCounterTimer();   
        }
        
        private void UpdateCounterTimer()
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;

                int mins = Mathf.FloorToInt(_remainingTime / 60);
                int secs = Mathf.FloorToInt(_remainingTime % 60);
                int milisecs = Mathf.FloorToInt((_remainingTime * 1000) % 1000);
                
               // timerText.SetText(
                    //string.Format("{0:D2}:{1:D2}:{2:D2}", mins, secs, milisecs)
                   // );
            }
            else
            {
                //LoadNextMinigame();
                Debug.Log("Timer is done!");
            }
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
            Debug.Log("Server says to start minigame and change timeScale!");
            if (message.StartMinigame)
            {
                Time.timeScale = 1;
            }
            
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
            Debug.Log("Receive " + message.TransitionName);
            
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

            Debug.Log(_ipField.text);

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
