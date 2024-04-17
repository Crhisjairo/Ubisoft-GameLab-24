using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
namespace _Scripts.Controllers.Boss
{
    public class Missile : NetworkBehaviour
    {
        public Transform target;
        public float speed = 10f;
        public float lifeTime = 5f;

        private float timer = 0f;

        void Update()
        {
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }

            timer += Time.deltaTime;

            if (timer >= lifeTime)
            {
                Destroy(gameObject);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        
        public float pushForce = 70f;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                Vector2 pushDirection = (other.transform.position - transform.position).normalized;
                pushDirection *= pushForce;
                
                other.GetComponent<PlayerController>().TakeDamage(pushDirection, 1);
                Destroy(gameObject);
            }
        }
    }
}
