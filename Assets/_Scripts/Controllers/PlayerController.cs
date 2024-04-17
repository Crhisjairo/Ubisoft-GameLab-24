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
        [SerializeField] public PlayerServerDataSync playerServerDataSync;

        [SerializeField] private GameObject normalBombPrefab;
        [SerializeField] private GameObject strongBombPrefab;
        [SerializeField] private Transform bombSpawnPoint;

        private BombType _currentBombType;

        private PlayerMovement _playerMovement;
        
        
        private bool _canFire = true;
        public float fireRateWaitTime = 0.3f;
        
        private void Awake()
        {
            playerServerDataSync = GetComponent<PlayerServerDataSync>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        [Command]
        public void CmdDie()
        {
            RpcDie();
        }
        
        public bool isDead = false;
        
        [ClientRpc]
        public void RpcDie()
        {
            _playerMovement.anim.SetTrigger(PlayerAnimations.OnDeath.ToString());
            _playerMovement.CmdSendTriggerAnimation(PlayerAnimations.OnDeath.ToString());
            _playerMovement.GetComponent<PlayerMovement>().MovementInput = Vector2.zero;
            _playerMovement.GetComponent<PlayerMovement>().rb.velocity = Vector2.zero;
            
            
            isDead = true;
        }
        
        public void TakeDamage(Vector2 impulseDirection, float amount)
        {
            // Check if dead
            var newHealth = playerServerDataSync.GetHealth();
            newHealth -= amount;
            
            StartCoroutine(_playerMovement.ActivateImpulseCounter(impulseDirection));
            _playerMovement.anim.SetTrigger(PlayerAnimations.Damaged.ToString());
            _playerMovement.CmdSendTriggerAnimation(PlayerAnimations.Damaged.ToString());

            // Notify server
            playerServerDataSync.CmdChangeHealth(newHealth);
            
            if (newHealth <= 0)
            {
                // Notify server
                CmdDie();
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
            switch (bombType)
            {
                case BombType.Regular:
                    GameObject normalBomb = Instantiate(normalBombPrefab, bombSpawnPoint.position, Quaternion.identity);
                    normalBomb.GetComponent<ThrowableBomb>().SetDirection(position);
                    
                    NetworkServer.Spawn(normalBomb);
                    // playerServerDataSync.SetRegularBombsServerSide();
                    break;
                case BombType.Strong:
                    if(playerServerDataSync.GetStrongBombs() <= 0) return;
                    
                    GameObject strongBomb = Instantiate(strongBombPrefab, bombSpawnPoint.position, Quaternion.identity);
                    strongBomb.GetComponent<ThrowableBomb>().SetDirection(position);
                    
                    NetworkServer.Spawn(strongBomb);
                    
                    var newStrongBombs = playerServerDataSync.GetStrongBombs();
                    newStrongBombs -= 1;

                    if (newStrongBombs <= 0)
                        newStrongBombs = 0;
                    playerServerDataSync.SetStrongBombsServerSide(newStrongBombs);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bombType), bombType, null);
            }
        }

        public void ThrowBomb(InputAction.CallbackContext context)
        {
            if(_playerMovement._currentMoveSetState == PlayerMoveSetStates.VerticalMove) return;
            
            if (!context.performed || !_canFire) return;
            if(isDead) return;
            
            StartCoroutine(StartCooldownTimer());
            
            if (playerServerDataSync.GetStrongBombs() <= 0)
            {
                _currentBombType = BombType.Regular;
            } else
            {
                _currentBombType = BombType.Strong;
            }
            
            _playerMovement.anim.SetTrigger(PlayerAnimations.OnAttack.ToString());
            _playerMovement.CmdSendTriggerAnimation(PlayerAnimations.OnAttack.ToString());
            
            // Notify server
            Vector2 position = _playerMovement.GetFacingDirection();
            
            CmdSpawnBomb(position, _currentBombType);
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
            HUDDebug.Instance.RemoveHealthListener((() => TakeDamage(Vector2.zero, 1)));
        }

    }
}
