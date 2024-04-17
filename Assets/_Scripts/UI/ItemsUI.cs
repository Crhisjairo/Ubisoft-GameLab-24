using System;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class ItemsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _regularBombText;
        [SerializeField] private TextMeshProUGUI _strongBombText;

        private void Start()
        {
            _strongBombText.text = "0";
        }

        public void UpdateRegularBombText(int amount)
        {
            _regularBombText.text = amount.ToString();
        }
        
        public void UpdateStrongBombText(int amount)
        {
            _strongBombText.text = amount.ToString();
        }
        
        
    }
}
