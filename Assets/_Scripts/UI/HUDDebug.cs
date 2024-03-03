using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace _Scripts.UI
{
    public class HUDDebug : MonoBehaviour
    {
        [SerializeField] private Button addHealth;
        [SerializeField] private Button removeHealth;
        
        public static HUDDebug Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        
        }

        public void AddHealthListener(UnityAction action)
        {    
            addHealth.onClick.AddListener(action);
        }
        
        public void RemoveHealthListener(UnityAction action)
        {
            removeHealth.onClick.AddListener(action);
        }
    }
}
