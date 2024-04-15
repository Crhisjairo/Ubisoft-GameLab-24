using System.Collections;
using UnityEngine;
namespace _Scripts.UI
{
    public class TutorialBox : MonoBehaviour
    {
        private Vector2 _startPos;
        public Vector2 offset;
        
        public float animSpeed = 1f;
        
        public float animRotation = 5f;
        public float animRotationSpeed = 1f;
        
        private int _tutorialIndex = 0;
        
        private void Start()
        {
           
        }

        
        public void StartAnimation()
        {
            if (_tutorialIndex == 0)
            {
                _startPos = transform.position;
            
                transform.rotation = Quaternion.Euler(0, 0, -animRotation);
        
                LeanTween.rotateZ(gameObject, animRotation, animRotationSpeed)
                    .setEaseInOutSine()
                    .setLoopPingPong();

                StartCoroutine(StartAnimationRoutine());
            }
            else
            {
                // Add the second tutorial here.
            }
            
        }

        private IEnumerator StartAnimationRoutine()
        {
            LeanTween.moveY(gameObject, transform.position.y + offset.y, animSpeed)
                .setEaseInOutBack();
            
            yield return new WaitForSeconds(animSpeed + 6);

            LeanTween.moveY(gameObject, transform.position.y - offset.y, animSpeed)
                .setEaseInOutBack();
            
        }

    }
}
