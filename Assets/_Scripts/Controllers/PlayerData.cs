using Mirror;
using UnityEngine;
namespace _Scripts.Controllers
{
    public class PlayerData : NetworkBehaviour
    {
        [SyncVar]
        public float maxHealth = 9f;
        [SyncVar]
        public float health = 5f;
        [SyncVar]
        public int regularBombs = int.MaxValue;
        [SyncVar]
        public int strongBombs = 0;
    }
}
