
using System;
using UnityEngine;
namespace _Scripts.Controllers.Bombs
{
    public class StrongThrowableBomb : ThrowableBomb
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            
            if(other.CompareTag("Boss"))
            {
                //other.GetComponent<Boss.Boss>().TakeDamage(10);
            }
        }
    }
}
