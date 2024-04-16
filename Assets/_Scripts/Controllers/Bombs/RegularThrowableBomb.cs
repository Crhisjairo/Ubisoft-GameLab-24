using UnityEngine;
namespace _Scripts.Controllers.Bombs
{
    public class RegularThrowableBomb : ThrowableBomb
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            
            if(other.CompareTag("Boss"))
            {
                //other.GetComponent<Boss.Boss>().TakeDamage(5);
            }
        }
    }
}
