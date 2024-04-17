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
        
        public CanvasGroup _tutorial1;
        public CanvasGroup _tutorial2;
        
        private void Start()
        {
           
        }

        
        public void StartAnimation()
        {
            _startPos = transform.position;
            
            transform.rotation = Quaternion.Euler(0, 0, -animRotation);
        
            LeanTween.rotateZ(gameObject, animRotation, animRotationSpeed)
                .setEaseInOutSine()
                .setLoopPingPong();

            StartCoroutine(StartAnimationRoutine());
            
            if (_tutorialIndex == 0)
            {
                _tutorial1.alpha = 1;
                _tutorial2.alpha = 0;
            }
            else
            {
                _tutorial1.alpha = 0;
                _tutorial2.alpha = 1;
                // Add the second tutorial here.
            }
            
            
            _tutorialIndex++;
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
