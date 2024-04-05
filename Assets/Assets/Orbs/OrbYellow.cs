using UnityEngine;
namespace Assets.Orbs
{
    public class OrbYellow : Orb
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Yellow orb is not implemented yet.");
            }
        }
    }
}