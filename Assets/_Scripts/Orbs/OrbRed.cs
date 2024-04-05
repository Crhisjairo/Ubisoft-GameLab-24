using UnityEngine;
using _Scripts.Controllers;

public class OrbRed : Orb
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