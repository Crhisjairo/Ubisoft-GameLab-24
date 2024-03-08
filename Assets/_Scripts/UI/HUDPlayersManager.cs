using _Scripts.UI.PlayerUIs;
using UnityEngine;

namespace _Scripts.UI
{
    public class HUDPlayersManager : MonoBehaviour
    {
        public PlayerUI player1UI;
        public PlayerUI player2UI;
        
        public static HUDPlayersManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
    }
}
