using System.Collections;
using UnityEngine;
namespace _Scripts.Controllers.Bombs
{
    public class ThrowableBomb : MonoBehaviour
    {
        public float speed = 7f;
        public float lifeTime = 3f;
        public float gravityScale = 0.05f;
        public GameObject explosionPrefab;

        private Rigidbody2D rb;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            // Apply initial velocity based on player's direction
            Vector2 direction = transform.right * (Mathf.Approximately(transform.localScale.x, 1) ? 1 : -1); // Check if player is facing right or left
            rb.velocity = direction * speed;

            rb.gravityScale = gravityScale;

            // Destroy the bullet after 'lifeTime' seconds
            Destroy(gameObject, lifeTime);
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Boss") || other.CompareTag("Player"))
            {
                StartCoroutine(explode());
            }
        }
        private IEnumerator explode()
        {
            //Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
