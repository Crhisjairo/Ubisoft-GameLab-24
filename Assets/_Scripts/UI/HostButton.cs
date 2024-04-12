using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace _Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class HostButton : NetworkBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        public string textOnClient = "Waiting for host to start...";
        public string textOnHost = "Waiting for dreamers...";

        public string startGameText = "Start to Dream!";

        private Coroutine animateTextCoroutine;


        private void Start()
        {
            hostButton.interactable = false;
            buttonText.text = textOnHost;
         
            if (!isServer)
            {
                buttonText.text = textOnClient;
            }
        }

        private void Update()
        {
            if (NetworkManager.singleton.numPlayers > 1)
            {
                hostButton.interactable = true;
                buttonText.text = startGameText;
                //modificar boton aqui
                StartCoroutine(AnimateButtonScale());
            }
            else if (NetworkManager.singleton.numPlayers < 2)
            {
                hostButton.interactable = false;
                buttonText.text = textOnHost;
            }
        }

        private IEnumerator AnimateButtonScale()
        {
            float duration = 1.0f; 
            Vector3 originalScale = hostButton.transform.localScale; 
            Vector3 targetScale = originalScale * 1.2f; 

            while (true)
            {
                for (float t = 0; t < duration; t += Time.deltaTime)
                {
                    hostButton.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
                    yield return null;
                }

                for (float t = 0; t < duration; t += Time.deltaTime)
                {
                    hostButton.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
                    yield return null;
                }
            }
        }
    }
}