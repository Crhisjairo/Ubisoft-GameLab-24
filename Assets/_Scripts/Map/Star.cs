using UnityEngine;
namespace _Scripts.Map
{
    public class Star : FlashingLight
    {
        public float animSpeed = 1f;

        [SerializeField] private Animator animator;
        public override void Start()
        {
            base.Start();
            animator.speed = animSpeed;
        }
    }
}
