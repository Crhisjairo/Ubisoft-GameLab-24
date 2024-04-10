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
