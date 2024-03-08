using UnityEngine;

namespace _Scripts.Map
{
    public class ObjectScroller : MonoBehaviour
    {
        public float speed = 5;
        
        public Vector2 direction = Vector2.left;
        
        
        // Update is called once per frame
        void Update()
        {
            transform.Translate(direction * (speed * Time.deltaTime));
        }
    }
}
