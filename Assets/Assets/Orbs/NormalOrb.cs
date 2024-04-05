using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using UnityEngine;
namespace Assets.Orbs
{
    public class NormalOrb : Orb
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
                
                player.AddHealth(1);
                
                Destroy(gameObject);
            }
        }
    }
}