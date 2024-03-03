using _Scripts.Controllers;
using _Scripts.Shared;
using Mirror;
using UnityEngine;

namespace _Scripts.Managers.Multiplayer
{
    public class MultiplayerManager : NetworkManager
    {
        // Client side
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Connected to server!");
        }
    
        // Server side
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("Player joined! numPlayers: " + numPlayers);

            if(numPlayers > 2) Debug.LogError("Too many players!");

            if(numPlayers == 1)
                SetPlayerNumber(conn.identity, PlayerIndex.Player1);
            else if(numPlayers == 2)
                SetPlayerNumber(conn.identity, PlayerIndex.Player2);
            
            
        }

        private void SetPlayerNumber(NetworkIdentity identity, PlayerIndex playerIndex)
        {
            PlayerServerDataSync clientServerData  = identity.GetComponent<PlayerServerDataSync>();
            
            clientServerData.SetPlayerIndex(playerIndex);
        }
        
    }
}
