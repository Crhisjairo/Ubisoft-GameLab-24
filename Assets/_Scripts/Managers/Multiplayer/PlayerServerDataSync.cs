using System;
using _Scripts.Controllers;
using _Scripts.ScriptableObjects;
using _Scripts.Shared;
using _Scripts.UI;
using _Scripts.UI.PlayerUIs;
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
        [SerializeField] PlayerDataScriptableObject playerDataScriptableObject;
        
        public Sprite[] playerSprites;
        public RuntimeAnimatorController[] playerAnimators;
        
        public SpriteRenderer SpriteRenderer { private set; get; }
        public PlayerUI PlayerUI { private set; get; }

        #region Specs
        [SyncVar(hook = nameof(OnPlayerIndexUpdate))] // hook -> this method will be executed on client side when server sends the new value for this variable.
        private PlayerIndex playerIndex = PlayerIndex.NotAssigned;

        [SyncVar(hook = nameof(OnMaxHealthChanged))] 
        private float maxHealth;
        
        // [SyncVar] -> Syncs the variable from server to client. Can be just modified by server. If clients modifies its own value, the server will not be notified.
        [SyncVar(hook = nameof(OnHealthChanged))] 
        private float health;
        
        
        [SyncVar(hook = nameof(OnRegularBombsChanged))]
        private int regularBombs = int.MaxValue;
        
        [SyncVar(hook = nameof(OnStrongBombsChanged))]
        private int strongBombs = 0;
        
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
            health = amount;
            maxHealth = amount;
        }
        
        [Server]
        public void SetStrongBombsServerSide(int amount)
        {
            strongBombs = amount;
        }
        
        [Server]
        public void SetRegularBombsServerSide(int amount)
        {
            regularBombs = amount;
        }

        [Server]
        public void SetPlayerIndex(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
        
        [Server]
        public void LoadDefaultPlayerData()
        {
            Debug.Log(playerDataScriptableObject.maxHealth);
            
            maxHealth = playerDataScriptableObject.maxHealth;
            health = playerDataScriptableObject.health;
            //TODO: Other settings must be loaded from here
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
        
        [Command]
        public void CmdChangeRegularBombs(int amount)
        {
            SetRegularBombsServerSide(amount);
        }
        
        [Command]
        public void CmdChangeStrongBombs(int amount)
        {
            SetStrongBombsServerSide(amount);
        }
        
        #endregion
        
        #region ClientSide
        
        
        public void OnHealthChanged(float oldValue, float newValue)
        {
            Debug.Log("Client: Health changed from " + oldValue + " to " + newValue);
            
            health = newValue;
            PlayerUI.lifeUIController.AddHeart((int)newValue); // TODO: Must be changed to float
            
        }
        
        public void OnMaxHealthChanged(float oldValue, float newValue)
        {
            Debug.Log("Client: Max Health changed from " + oldValue + " to " + newValue);
            
            maxHealth = newValue;
            PlayerUI.lifeUIController.SetMaxHeartsTo((int)newValue); // TODO: Must be changed to float
            
        }
        
        public void OnRegularBombsChanged(int oldValue, int newValue)
        {
            regularBombs = newValue;
            
            HUDPlayersManager.Instance.globalItemUI.UpdateRegularBombText(newValue);
        }

        public void OnStrongBombsChanged(int oldValue, int newValue)
        {
            Debug.Log("Strong bombs changed from " + oldValue + " to " + newValue);
            
            strongBombs = newValue;
            
            HUDPlayersManager.Instance.globalItemUI.UpdateStrongBombText(newValue);
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
                Debug.Log("Player 1 Sprite :" + SpriteRenderer.sprite.name);
                GetComponent<PlayerMovement>().anim.runtimeAnimatorController = playerAnimators[0];
                
                PlayerUI = HUDPlayersManager.Instance.player1UI;
                Debug.Log("OnPlayerNumberUpdate: " + name + " with UI: " + PlayerUI.name);
            }
            else if(newPlayerIndex == PlayerIndex.Player2)
            {
                // Set player 2 specs
               SpriteRenderer.sprite = playerSprites[1];
               Debug.Log("Player 2 Sprite :" + SpriteRenderer.sprite.name);
               PlayerUI = HUDPlayersManager.Instance.player2UI;
               GetComponent<PlayerMovement>().anim.runtimeAnimatorController = playerAnimators[1];
               
               Debug.Log("OnPlayerNumberUpdate: " + name + " with UI: " + PlayerUI.name);
            }
        }
        
        #endregion
        
        public float GetHealth()
        {
            return health;
        }
        
        public float GetMaxHealth()
        {
            return maxHealth;
        }
        
        public PlayerIndex GetPlayerIndex()
        {
            return playerIndex;
        }
        
        public int GetRegularBombs()
        {
            return regularBombs;
        }
        
        public int GetStrongBombs()
        {
            return strongBombs;
        }
    }
}
