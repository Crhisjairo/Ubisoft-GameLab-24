using System;
using System.Collections;
using UnityEngine;
namespace _Scripts.Controllers.Bombs
{
    public class ThrowableBomb : MonoBehaviour
    {
        public float speed = 25f;
        public float lifeTime = 8f;
        public float gravityScale = 0.05f;
        public GameObject explosionPrefab;
        
        public float waitTimeBeforeGravity = 2f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        protected virtual void Start()
        {
            
            // Destroy the bullet after 'lifeTime' seconds
            Invoke(nameof(ExplodeInvocation), lifeTime);
        }

        private void ExplodeInvocation()
        {
            StartCoroutine(Explode());
        }

        private IEnumerator ApplyGravityRoutine()
        {
            yield return new WaitForSeconds(waitTimeBeforeGravity);
            
            rb.gravityScale = gravityScale;
        }

        public void SetDirection(Vector2 direction)
        {
            Debug.Log("SetDirection: " + direction);
            
            rb.velocity = direction * speed;
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Boss") || other.CompareTag("Player"))
            {
                ExplodeInvocation();
            }
        }
        
        private IEnumerator Explode()
        {
            //Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
