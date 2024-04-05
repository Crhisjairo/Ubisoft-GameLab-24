using UnityEngine;
using _Scripts.Controllers;

public class Debuff : Orb
{
    // Override the Awake method to set a different gravity scale
    protected override void Awake()
    {
        gravityScale = 0.22f;
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(0.5f);
                Destroy(gameObject);
            }
        }
    }
}
