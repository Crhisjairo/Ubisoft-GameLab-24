using UnityEngine;
namespace _Scripts.Controllers
{
    public class PlayerData : MonoBehaviour
    {
        public float maxHealth = 9f;
        public float health = 5f;
        public int regularBombs = int.MaxValue;
        public int strongBombs = 0;
    }
}
