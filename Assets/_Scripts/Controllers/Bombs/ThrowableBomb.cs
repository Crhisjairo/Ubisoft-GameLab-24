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
        
        public int damage = 3;

        private Rigidbody2D rb;
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _collider;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
        }

        protected virtual void Start()
        {
            
            // Destroy the bullet after 'lifeTime' seconds
            Invoke(nameof(ExplodeInvocation), lifeTime);
            Invoke(nameof(ApplyGravityInvocation), 0);
        }

        private void ExplodeInvocation()
        {
            StartCoroutine(Explode());
        }
        
        private void ApplyGravityInvocation()
        {
            StartCoroutine(ApplyGravityRoutine());
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
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Boss"))
            {
                ExplodeInvocation();
            }
        }
        
        private IEnumerator Explode()
        {
            //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            _spriteRenderer.enabled = false;
            _collider.enabled = false;

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
