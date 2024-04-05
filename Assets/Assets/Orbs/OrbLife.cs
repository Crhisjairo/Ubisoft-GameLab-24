using _Scripts.Controllers;
using UnityEngine;
namespace Assets.Orbs
{
    public class OrbLife : Orb
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            return;
            if (other.CompareTag("Player"))
            {
                PlayerController player = null;

                other.TryGetComponent<PlayerController>(out player);

                if (player == null)
                {
                    // Maybe is a basket
                    Basket basket = null;
                    other.TryGetComponent<Basket>(out basket);
                    player = basket.GetGlobalPlayerData();
                }

                if (player == null)
                {
                    Debug.LogWarning("Player not found when " + nameof(OrbLife) + " collided with " + other.name);
                    return;
                }
                
                player.AddHealth(1);
                
                Destroy(gameObject);
            }
        }
    }
}