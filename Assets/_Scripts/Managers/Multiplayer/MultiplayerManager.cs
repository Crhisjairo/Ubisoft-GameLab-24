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
        
        private MinigameSceneNames _currentMinigameScene = MinigameSceneNames.Empty;

        private bool _isClientInTransition;
        private bool firstSceneLoaded;
        
        private bool subscenesLoaded;
        
        private Queue<AsyncOperation> _scenesAsyncOperations = new Queue<AsyncOperation>(0);

        public override void Start()
        {
            base.Start();
            
            NetworkClient.RegisterHandler<ServerLoadSceneMessage>(OnLoadSceneMessage);
        }

        
        /// <summary>
        /// On client side when the server sends a message to load a scene.
        /// </summary>
        /// <param name="msg"></param>
        private void OnLoadSceneMessage(ServerLoadSceneMessage msg)
        {
        }
        
        // Client side
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Connected to server!");
        }
    
        // Server side
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("Player joined! numPlayers: " + numPlayers);

            if(numPlayers > 2) Debug.LogError("Too many players!");

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
            }
            
            subscenesLoaded = true;
        }
        
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            // When client has loaded the online scene: Minigames_Scenes.
            base.OnServerReady(conn);
            
            Debug.Log("Client loaded online scene!");
            
            if (conn.identity == null) // Notify players to load the current minigame.
                StartCoroutine(AddPlayerDelayed(conn, "Minigame_1")); // TODO: change by _scenesSortedRan.Dequeue().ToString()...
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
            
            conn.Send(new SceneMessage
            {
                sceneName = sceneNameToLoad,
                sceneOperation = SceneOperation.LoadAdditive,
                customHandling = true
            });

            Transform startPos = GetStartPosition(); // Get the start position for the player.

            GameObject player = Instantiate(playerPrefab, startPos);
            player.transform.SetParent(null);

            yield return new WaitForEndOfFrame();
            
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneNameToLoad));

            NetworkServer.AddPlayerForConnection(conn, player);
        }

        [ServerCallback]
        private IEnumerator SendPlayerToNewScene(GameObject player)
        {
            yield return null;
        }
        
        private IEnumerator LoadAdditive(string sceneName)
        {
            _isClientInTransition = true;

            yield return canvasGroupFade.FadeIn();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                
                while(loadingSceneAsync != null && !loadingSceneAsync.isDone)
                {
                    yield return null;
                }
            }

            NetworkClient.isLoadingScene = false;
            _isClientInTransition = false;
            
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
            
            OnClientSceneChanged();
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            Debug.Log("Client changed scene to: " + newSceneName);
            
            if (sceneOperation == SceneOperation.UnloadAdditive)
                StartCoroutine(UnloadAdditive(newSceneName));

            if (sceneOperation == SceneOperation.LoadAdditive)
                StartCoroutine(LoadAdditive(newSceneName));
        }
        
        public override void OnClientSceneChanged()
        {
            //When players finish loading the scene.
            if (!_isClientInTransition)
            {
                base.OnClientSceneChanged();   
            }
        }
        
        private void SetPlayerNumber(NetworkIdentity identity, PlayerIndex playerIndex)
        {
            PlayerServerDataSync clientServerData  = identity.GetComponent<PlayerServerDataSync>();
            
            clientServerData.SetPlayerIndex(playerIndex);
            clientServerData.LoadDefaultPlayerData();
            
        }


    }
}
