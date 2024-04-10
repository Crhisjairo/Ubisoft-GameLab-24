using System;
using _Scripts.Managers.Multiplayer;
using _Scripts.ScriptableObjects;
using _Scripts.Shared;
using _Scripts.UI;
using _Scripts.UI.PlayerUIs;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Controllers
{
    /// <summary>
    /// This class is responsible for controlling player's properties and notifying server when something changes.
    /// </summary>
    [RequireComponent(typeof(PlayerServerDataSync))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class PlayerController : NetworkBehaviour
    {
        [FormerlySerializedAs("_playerDataSync")]
        [SerializeField] PlayerServerDataSync playerServerDataSync;

        private void Awake()
        {
            playerServerDataSync = GetComponent<PlayerServerDataSync>();
        }

        public void TakeDamage(float amount)
        {
            // Check if dead
            var newHealth = playerServerDataSync.GetHealth();
            newHealth -= amount;

            // Notify server
            playerServerDataSync.CmdChangeHealth(newHealth);
            
            if (newHealth <= 0)
            {
                // Notify server
                //playerServerDataSync.CmdDie();
            }
        }
        
        public void AddHealth(float amount)
        {
            //Check if full health
            var newHealth = playerServerDataSync.GetHealth();
            
            if(newHealth >= playerServerDataSync.GetMaxHealth()) return;
            
            newHealth += amount;
            
            // Notify server
            playerServerDataSync.CmdChangeHealth(newHealth);
        }
        
        public void AddHeartSlot(float amount)
        {
            var newMaxHealth = playerServerDataSync.GetMaxHealth();
            newMaxHealth += amount;
            
            playerServerDataSync.CmdChangeMaxHealth(newMaxHealth);
        }

        public void AddRegularBombs(int amount)
        {
            var newRegularBombs = playerServerDataSync.GetRegularBombs();
            newRegularBombs += amount;
            
            playerServerDataSync.CmdChangeRegularBombs(newRegularBombs);
        }
        
        public void AddStrongBombs(int amount)
        {
            var newStrongBombs = playerServerDataSync.GetStrongBombs();
            newStrongBombs += amount;
            
            playerServerDataSync.CmdChangeStrongBombs(newStrongBombs);
        }

        public void RemoveStrongBombs(int amount)
        {
            var newStrongBombs = playerServerDataSync.GetStrongBombs();
            newStrongBombs -= amount;

            if (newStrongBombs <= 0)
                newStrongBombs = 0;
            
            playerServerDataSync.CmdChangeStrongBombs(newStrongBombs);
        }

        private void OnEnable()
        {
            // JUST FOR DEBUGGING. THIS CAUSE A WARNING ON CONSOLE. Anyways, this do not cause any problem.
            if(HUDDebug.Instance == null) return;
            
            HUDDebug.Instance.AddHealthListener(() => AddHealth(1));
            HUDDebug.Instance.RemoveHealthListener((() => TakeDamage(1)));
        }

    }
}
