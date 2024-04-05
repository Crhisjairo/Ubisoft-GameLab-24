using _Scripts.Controllers;
using UnityEngine;
namespace Assets.Orbs
{
    public class OrbLife : Orb
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                player.AddHealth(1);
                Destroy(gameObject);
            }
        }
    }
}