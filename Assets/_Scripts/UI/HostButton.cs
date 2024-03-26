using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class HostButton : NetworkBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        public string textOnClient = "Waiting for host to start...";
        
        private void Start()
        {
            if (!isServer)
            {
                hostButton.interactable = false;
                buttonText.text = textOnClient;
            }
        }
    }
}