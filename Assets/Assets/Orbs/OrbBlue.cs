using UnityEngine;
using _Scripts.Controllers;

public class OrbBlue : Orb
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //todo:add missile bullets
            Destroy(gameObject);
        }
    }
}