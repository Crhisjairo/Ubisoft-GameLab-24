using System;
using _Scripts.Shared;
using _Scripts.UI;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Managers.Multiplayer
{
    /// <summary>
    /// This class is responsible for syncing player data between server and clients. Logic for data must be validated on PlayerController class.
    /// </summary>
    public class PlayerServerDataSync : NetworkBehaviour
    {
        public Sprite[] playerSprites;
        public SpriteRenderer SpriteRenderer { private set; get; }
        public PlayerUI PlayerUI { private set; get; }

        public Transform[] spawnPoints; // Array of spawn points for players

        #region Specs
        [SyncVar(hook = nameof(OnPlayerIndexUpdate))] // hook -> this method will be executed on client side when server sends the new value for this variable.
        private PlayerIndex playerIndex = PlayerIndex.NotAssigned;

        [SyncVar(hook = nameof(OnMaxHealthChanged))] 
        [SerializeField] private float maxHealth;
        
        // [SyncVar] -> Syncs the variable from server to client. Can be just modified by server. If clients modifies its own value, the server will not be notified.
        [SyncVar(hook = nameof(OnHealthChanged))] 
        [SerializeField] private float health;
        
        
        #endregion
        
        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        #region ServerSide
        
        [Server]
        public void SetHealthServerSide(float amount)
        {
            health = amount; // This will trigger the hook on client side, because it's a SyncVar.
        }
        
        [Server]
        public void SetMaxHealthServerSide(float amount)
        {
            maxHealth = amount;
        }
        
        [Server]
        public void SetPlayerIndex(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
        
        [Command] // Command -> are methods that are called from client and executed on server.
        public void CmdChangeHealth(float amount)
        {
            Debug.Log("Cmd on server: Health changed to " + amount);
            // We can call a server method from here.
            SetHealthServerSide(amount);   
        }
        
        [Command]
        public void CmdChangeMaxHealth(float amount)
        {
            SetMaxHealthServerSide(amount);
        }
        
        #endregion
        
        #region ClientSide
        public void OnHealthChanged(float oldValue, float newValue)
        {
            Debug.Log("Client: Health changed from " + oldValue + " to " + newValue);
            
            health = newValue;
            PlayerUI.healthSlider.value = newValue;
            PlayerUI.healthText.text = "Health: " + health;
        }
        
        public void OnMaxHealthChanged(float oldValue, float newValue)
        {
            Debug.Log("Client: Max Health changed from " + oldValue + " to " + newValue);
            
            maxHealth = newValue;
            PlayerUI.healthSlider.maxValue = newValue;
            PlayerUI.maxHealthText.text = "Max: " + maxHealth;
        }
        
        /// <summary>
        /// Sets the player index and updates the player data.
        /// </summary>
        /// <param name="oldPlayerIndex"></param>
        /// <param name="newPlayerIndex"></param>
        public void OnPlayerIndexUpdate(PlayerIndex oldPlayerIndex, PlayerIndex newPlayerIndex)
        {
            Debug.Log("Player number updated from " + oldPlayerIndex + " to " + newPlayerIndex);
            
            if(newPlayerIndex == PlayerIndex.Player1)
            {
                // Set player 1 specs
                SpriteRenderer.sprite = playerSprites[0];
                PlayerUI = HUDPlayersManager.Instance.player1UI;
                Debug.Log("OnPlayerNumberUpdate: " + name + " with UI: " + PlayerUI.name);
            }
            else if(newPlayerIndex == PlayerIndex.Player2)
            {
                // Set player 2 specs
               SpriteRenderer.sprite = playerSprites[1];
               PlayerUI = HUDPlayersManager.Instance.player2UI;
               Debug.Log("OnPlayerNumberUpdate: " + name + " with UI: " + PlayerUI.name);
            }
            
            CmdChangeMaxHealth(5);
            CmdChangeHealth(2); // TODO: default max health value
        }
        
        #endregion

        public float GetHealth()
        {
            return health;
        }
        
        public PlayerIndex GetPlayerIndex()
        {
            return playerIndex;
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            if (isLocalPlayer)
            {
                // Get the spawn point based on player index
                Transform spawnPoint = GetSpawnPoint(GetPlayerIndex());

                // Set the player's position to the spawn point
                transform.position = spawnPoint.position;
            }
        }

        private Transform GetSpawnPoint(PlayerIndex playerIndex)
        {
            // Return the appropriate spawn point based on the player index
            if (playerIndex == PlayerIndex.Player1)
            {
                return spawnPoints[0];
            }
            else if (playerIndex == PlayerIndex.Player2)
            {
                return spawnPoints[1];
            }
            else
            {
                Debug.LogError("Invalid player index!");
                return null;
            }
        }
    }
}
