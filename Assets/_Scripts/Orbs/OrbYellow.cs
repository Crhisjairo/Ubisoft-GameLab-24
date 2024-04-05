using UnityEngine;
using _Scripts.Controllers;

public class OrbYellow : Orb
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}