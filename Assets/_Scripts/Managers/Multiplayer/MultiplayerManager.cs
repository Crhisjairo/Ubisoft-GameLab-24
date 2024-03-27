using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers.Multiplayer.Messages;
using _Scripts.Shared;
using Assets._Scripts.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _Scripts.Managers.Multiplayer
{
    public class MultiplayerManager : NetworkManager
    {
        [SerializeField] private string firstSceneToLoad;
        
        [FormerlySerializedAs("sceneLoader")]
        [SerializeField] private CanvasGroupFade canvasGroupFade;
        
        [SerializeField] private MinigameSceneNames[] scenesNamesToLoad;
        private Queue<MinigameSceneNames> _scenesSortedRan;


        private bool _isClientInTransition;
        private bool firstSceneLoaded;

        private bool subscenesLoaded;

        private Queue<AsyncOperation> _scenesAsyncOperations = new Queue<AsyncOperation>(0);
        private Queue<string> _sortedScenesNames = new Queue<string>(0);

        private int _playerWithScenesLoaded = 0;

        private AsyncOperation _clientCurrentMinigameScene;
        private string _clientCurrentMinigameSceneName;
        
        public override void Start()
        {
            base.Start();
            
            NetworkServer.RegisterHandler<SceneStatusMessage>(OnServerSceneStatusReceive);
            NetworkClient.RegisterHandler<ServerActivateSceneMessage>(OnClientActivateSceneReceive);
        }
      

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            // When client has loaded the online scene: Minigames_Scenes.
            base.OnServerReady(conn);
            
            Debug.Log("Client loaded Minigames_Scenes scene!");

            //if (conn.identity == null) // Notify players to load the current minigame.
                //StartCoroutine(AddPlayerDelayed(conn, "Minigame_1")); // TODO: change by _scenesSortedRan.Dequeue().ToString()...
        }
        
        IEnumerator AddPlayerDelayed(NetworkConnectionToClient conn, string sceneNameToLoad)
        {
            while (!subscenesLoaded)
                yield return null;

            NetworkIdentity[] allObjsWithANetworkIdentity = FindObjectsOfType<NetworkIdentity>();

            foreach (var obj in allObjsWithANetworkIdentity)
            {
                obj.enabled = true;
            }
            
            firstSceneLoaded = false;
            
            // Notify the client to load the scene.
            conn.Send(new SceneMessage
            {
                sceneName = sceneNameToLoad,
                sceneOperation = SceneOperation.LoadAdditive,
                customHandling = true
            });

            // Adding the player to the scene on Server.
            Transform startPos = GetStartPosition(); // Get the start position for the player.
            GameObject player = Instantiate(playerPrefab, startPos);
            player.transform.SetParent(null);

            yield return new WaitForEndOfFrame();
            
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneNameToLoad));

            NetworkServer.AddPlayerForConnection(conn, player);
        }
        
        /// <summary>
        /// Ejecutado en el cliente cuando el servidor manda un SceneMessage.
        /// </summary>
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            Debug.Log("Client notified to change scene to: " + newSceneName);
            
            if (sceneOperation == SceneOperation.UnloadAdditive)
                StartCoroutine(UnloadAdditive(newSceneName));

            if (sceneOperation == SceneOperation.LoadAdditive)
                StartCoroutine(LoadAdditive(newSceneName));
        }

        
        [ServerCallback]
        private void OnServerSceneStatusReceive(NetworkConnection conn, SceneStatusMessage msg)
        {
            Debug.Log("Server received scene status message: " + msg.SceneName + " is ready: " + msg.IsReady + " from: " + conn.identity);
            _playerWithScenesLoaded++;

            Debug.Log("_playerWithScenesLoaded: " + _playerWithScenesLoaded);

            /*
             * Normally, _playerWithScenesLoaded will be 4, but because the host will only send the message when minigame_1 is loaded.
             * Host only send message when minigame_1 is loaded, but the other players send messages for the online scene and minigame_1  
             */
            if (_playerWithScenesLoaded == 3)
            {
                // Here, the two players are ready to start the minigame.
                
                // Server
                _scenesAsyncOperations.Dequeue().allowSceneActivation = true;
                // Clients
                NetworkServer.SendToAll(new ServerActivateSceneMessage
                {
                    SceneName = _sortedScenesNames.Dequeue(),
                    Activate = true
                } );
            }
            
        }
        
         
        private void OnClientActivateSceneReceive(ServerActivateSceneMessage msg)
        {
            Debug.Log("Clientes deben activar sus escenas");
            
           //_clientCurrentMinigameScene.allowSceneActivation = true;
        }

        private void SetPlayerNumber(NetworkIdentity identity, PlayerIndex playerIndex)
        {
            PlayerServerDataSync clientServerData  = identity.GetComponent<PlayerServerDataSync>();
            
            clientServerData.SetPlayerIndex(playerIndex);
            clientServerData.LoadDefaultPlayerData();
            
        }
        
        // Server side
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("Player joined! numPlayers: " + numPlayers);

            if(numPlayers > 2) Debug.LogError("Too many players!");

            // Player is notified on SetPlayerNumber method.
            if(numPlayers == 1)
                SetPlayerNumber(conn.identity, PlayerIndex.Player1);
            else if(numPlayers == 2)
                SetPlayerNumber(conn.identity, PlayerIndex.Player2);
        }

        /// <summary>
        /// Load the subscenes after the online scene is loaded.
        /// </summary>
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            
            if (sceneName == onlineScene)
            {
               StartCoroutine(ServerLoadSubScenes());
            }
        }
        
        
        /// <summary>
        /// Llamado en el cliente cuando termina de cargar la escena que ha sido empezada en el servidor.
        /// </summary>
        public override void OnClientSceneChanged()
        {
            //When players finish loading the scene.
            if (!_isClientInTransition)
            {
                base.OnClientSceneChanged();   
                
                // ACA notificar al servidor que la escena fue cargada.
                NetworkClient.Send(new SceneStatusMessage
                {
                    SceneName = _clientCurrentMinigameSceneName,
                    IsReady = true
                });
            }
        }
       
        
        private IEnumerator LoadAdditive(string sceneName)
        {
            
            Debug.Log("Starting loading: " + sceneName);
            _isClientInTransition = true;

            yield return canvasGroupFade.FadeIn();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                _clientCurrentMinigameScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                //_clientCurrentMinigameScene.allowSceneActivation = false;
                
                while(_clientCurrentMinigameScene != null && !_clientCurrentMinigameScene.isDone)
                {
                    yield return null;
                }
            }
            
            
            Debug.Log("Finished loading: " + sceneName);

            NetworkClient.isLoadingScene = false;
            _isClientInTransition = false;

            _clientCurrentMinigameSceneName = sceneName;
            
            OnClientSceneChanged();

            if (firstSceneLoaded == false)
            {
                firstSceneLoaded = true;

                yield return new WaitForSeconds(0.06f);
            }
            else
            {
                firstSceneLoaded = true;

                yield return new WaitForSeconds(0.05f);
            }
            
            yield return canvasGroupFade.FadeOut();
        }

        private IEnumerator UnloadAdditive(string sceneName)
        {
            _isClientInTransition = true;
            
            yield return canvasGroupFade.FadeIn();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return Resources.UnloadUnusedAssets();
            }
            
            NetworkClient.isLoadingScene = false;
            _isClientInTransition = false;
            
            //OnClientSceneChanged();
        }
        
        
        /// <summary>
        /// Loads minigames subscenes on server.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ServerLoadSubScenes()
        {
            yield return canvasGroupFade.FadeIn();
            
            foreach (var scene in scenesNamesToLoad)
            {
                AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), new LoadSceneParameters
                {
                    loadSceneMode = LoadSceneMode.Additive,
                    localPhysicsMode = LocalPhysicsMode.Physics2D
                });
                
                sceneAsync.allowSceneActivation = false;
                
                _scenesAsyncOperations.Enqueue(sceneAsync);
                _sortedScenesNames.Enqueue(scene.ToString());
            }
            
            subscenesLoaded = true;
        }

        [ServerCallback]
        private IEnumerator SendPlayerToNewScene(GameObject player)
        {
            yield return null;
        }


    }
}
