using System;
using _Scripts.Map;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.PlayerUIs
{
    
    public class PlayerSlot : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetIsReadyText))]
        public bool isReady = false;
        
        [SerializeField] private TextMeshProUGUI _readyText;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private FloatingAnim _nameFloatingAnim;
        [SerializeField] private Image _playerImage;
        
        [SerializeField] private float imageAlphaWhenNotReady = 0.3f;

        private void Start()
        {
            _playerAnimator.enabled = false;
            _nameFloatingAnim.enabled = false;
            
            _playerImage.color = new Color(_playerImage.color.r, _playerImage.color.g, _playerImage.color.b, imageAlphaWhenNotReady);
        }

        public void SetIsReadyText(bool oldValue, bool newValue)
        {
            _readyText.text = newValue ? "Ready to dream!" : "Not Ready";
            
            _playerAnimator.enabled = newValue;
            _nameFloatingAnim.enabled = newValue;

            if (newValue)
                _playerImage.color = new Color(_playerImage.color.r, _playerImage.color.g, _playerImage.color.b, 1);
            else
                _playerImage.color = new Color(_playerImage.color.r, _playerImage.color.g, _playerImage.color.b, imageAlphaWhenNotReady);

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
