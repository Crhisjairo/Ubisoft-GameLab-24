using System.Collections;
using Assets._Scripts.Shared;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Multiplayer
{
    public class ScenesLoader: NetworkBehaviour
    {
        private bool _isClientInTransition = false;

        public void LoadScenes(string name)
        {
            //StartCoroutine(LoadAdditive(name));

            
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (isClient)
            {
                NetworkManager.singleton.StopClient();
            }

            SceneManager.LoadScene(name);
        }
        
        private IEnumerator LoadAdditive(string sceneName)
        {
            
            Debug.Log("Starting loading: " + sceneName);
            _isClientInTransition = true;

            //yield return canvasGroupFade.FadeIn();

            AsyncOperation _clientCurrentMinigameScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            
            while(!_clientCurrentMinigameScene.isDone)
            {
                yield return null;
            }
            
            
            Debug.Log("Finished loading: " + sceneName);

            _isClientInTransition = false;
            Destroy(gameObject);

            //yield return canvasGroupFade.FadeOut();
            yield return null;
        }

    }
}
