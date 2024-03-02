using System;
using _Scripts.Managers.Multiplayer;
using _Scripts.Shared;
using _Scripts.UI;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.Controllers
{
    /// <summary>
    /// This class is responsible for controlling a player.
    /// </summary>
    [RequireComponent(typeof(PlayerDataSync))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerDataSync _playerDataSync;
        
        private void Awake()
        {
            _playerDataSync = GetComponent<PlayerDataSync>();
        }

        public void TakeDamage(float amount)
        {
            // Check if dead
            var newHealth = _playerDataSync.GetHealth();
            newHealth -= amount;
            
            
            
            _playerDataSync.NotifyServerHealthChange(newHealth);
        }
        
        
        
        public void AddHealth(float amount)
        {
            //Check if full health
            var newHealth = _playerDataSync.GetHealth();
            newHealth += amount;
            
            _playerDataSync.NotifyServerHealthChange(newHealth);
        }
    
        private void OnEnable()
        {
            HUDDebug.Instance.AddHealthListener(() => AddHealth(1));
            HUDDebug.Instance.RemoveHealthListener((() => TakeDamage(1)));
        }
        
    }
}
