using UnityEngine;
namespace _Scripts.Map
{
    public class FloatingAnim : MonoBehaviour
    {
        public float animSpeed = 1f;
        public float animHeight = 0.5f;
        
        public float animRotation = 5f;
        public float animRotationSpeed = 1f;
        // Start is called before the first frame update
        void Start()
        {
            LeanTween.moveY(gameObject, transform.position.y + animHeight, animSpeed).setEaseInOutSine().setLoopPingPong(); 
            
            transform.rotation = Quaternion.Euler(0, 0, -animRotation);
            
            LeanTween.rotateZ(gameObject, animRotation, animRotationSpeed).setEaseInOutSine().setLoopPingPong();
        }

    }
}
