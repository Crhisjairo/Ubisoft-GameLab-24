using System;
using System.Collections;
using _Scripts.Controllers.Bombs;
using _Scripts.Managers.Multiplayer;
using _Scripts.ScriptableObjects;
using _Scripts.Shared;
using _Scripts.UI;
using _Scripts.UI.PlayerUIs;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

        [SerializeField] private GameObject normalBombPrefab;
        [SerializeField] private GameObject strongBombPrefab;

        private BombType _currentBombType;

        private PlayerMovement _playerMovement;
        
        
        private bool _canFire = true;
        public float fireRateWaitTime = 0.5f;
        
        private void Awake()
        {
            playerServerDataSync = GetComponent<PlayerServerDataSync>();
            _playerMovement = GetComponent<PlayerMovement>();
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

        [Command]
        public void CmdSpawnBomb(Vector2 position, BombType bombType)
        {
            Vector2 direction = _playerMovement.GetFacingDirection();
            
            switch (bombType)
            {
                case BombType.Regular:
                    GameObject normalBomb = Instantiate(normalBombPrefab, position, Quaternion.identity);
                    normalBomb.GetComponent<ThrowableBomb>().SetDirection(direction);
                    
                    NetworkServer.Spawn(normalBomb);
                    // playerServerDataSync.SetRegularBombsServerSide();
                    break;
                case BombType.Strong:
                    if(playerServerDataSync.GetStrongBombs() <= 0) return;
                    
                    GameObject strongBomb = Instantiate(strongBombPrefab, position, Quaternion.identity);
                    strongBomb.GetComponent<ThrowableBomb>().SetDirection(direction);
                    
                    NetworkServer.Spawn(strongBomb);
                    RemoveStrongBombs(1);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bombType), bombType, null);
            }
        }

        public void ThrowBomb(InputAction.CallbackContext context)
        {
            if (!context.performed || !_canFire) return;
            
            if (playerServerDataSync.GetStrongBombs() <= 0)
            {
                _currentBombType = BombType.Regular;
            }
            
            // Notify server
            // TODO: For testing purposes
            CmdSpawnBomb(transform.position, BombType.Strong);
        }
        
        private IEnumerator StartCooldownTimer()
        {
            _canFire = false;
            yield return new WaitForSeconds(fireRateWaitTime);
            _canFire = true;
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
