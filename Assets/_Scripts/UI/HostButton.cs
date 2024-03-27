using System;
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
            }
        }
    }
}