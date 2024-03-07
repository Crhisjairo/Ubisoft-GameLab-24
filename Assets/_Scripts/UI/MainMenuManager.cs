using Assets._Scripts.Shared;
using UnityEditor.Animations;
using UnityEngine;

namespace _Scripts.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        private Animator animator; // Example animator variable

        // Start is called before the first frame update
        void Start()
        {
            // Get the Animator component attached to this GameObject
            animator = GetComponent<Animator>();

            // Example: Play an animation
            if (animator != null)
            {
                animator.Play("YourAnimationName");
            }
        }
    }
}
