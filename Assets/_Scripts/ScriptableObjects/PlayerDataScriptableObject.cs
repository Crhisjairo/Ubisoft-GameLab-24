using UnityEngine;
namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DefaultPlayerData", menuName = "PlayerData/Default", order = 0)]
    public class PlayerDataScriptableObject : ScriptableObject
    {
        public float maxHealth = 9;
        public float health = 4;
        public float currentHealth = 5;
        
        public float attackDamage = 1;
        public float defense = 0;
        public float speed = 1;

    }
}
