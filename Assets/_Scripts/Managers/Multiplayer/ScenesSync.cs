using System.Collections;
using Assets._Scripts.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Multiplayer
{
    public class ScenesSync: NetworkBehaviour
    {
        private static MultiManager2 _multiManager2;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _multiManager2 = FindObjectOfType<MultiManager2>();
        }


        #region Server

        [Command]
        private void CmdStartGame()
        {
            if (_multiManager2.PlayersOnLobbyCount == 2)
            {
                // Notify all clients to change scene.
                RpcChangeScene(MinigameSceneNames.Minigame_1.ToString());
            }
            else
            {
                Debug.Log("Not all players are ready.");
            }
        }

        #endregion
        
        #region Client
        
        private bool _isClientInTransition;
        private string _clientCurrentMinigameSceneName;

        public void OnStartGameClick()
        {
            // Call the command on the server to try start the game.
            // if is local player
            CmdStartGame();
        }

        [ClientRpc]
        void RpcChangeScene(string sceneName)
        {
            //TODO: check if unload first
            StartCoroutine(LoadAdditive(sceneName));
        }

        private IEnumerator LoadAdditive(string sceneName)
        {
            
            Debug.Log("Starting loading: " + sceneName);
            _isClientInTransition = true;

            //yield return canvasGroupFade.FadeIn();

            if (_multiManager2.mode == NetworkManagerMode.ClientOnly)
            {
                AsyncOperation _clientCurrentMinigameScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                
                
                while(_clientCurrentMinigameScene != null && !_clientCurrentMinigameScene.isDone)
                {
                    yield return null;
                }
            }
            
            
            Debug.Log("Finished loading: " + sceneName);

            NetworkClient.isLoadingScene = false;
            _isClientInTransition = false;

            _clientCurrentMinigameSceneName = sceneName;
            
            //OnClientSceneChanged();

            //yield return canvasGroupFade.FadeOut();
            yield return null;
        }

        #endregion
    }
}
