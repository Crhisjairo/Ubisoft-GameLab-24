using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace _Scripts.UI.PlayerUIs
{
    
    public class PlayerSlot : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetIsReadyText))]
        public bool isReady = false;
        
        [SerializeField] private TextMeshProUGUI _readyText;

        public void SetIsReadyText(bool oldValue, bool newValue)
        {
            _readyText.text = newValue ? "Ready to dream!" : "Not Ready";

            Color color = _readyText.color;
            color.a = newValue ? 1 : 0.5f;
            _readyText.color = color;
            
            isReady = newValue;
        }

        public void SetIsReady(bool isReady)
        {
            if (isServer)
            {
                this.isReady = isReady;
            }
        }
    }
}
