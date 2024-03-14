using UnityEngine;

namespace _Scripts.Shared
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

    }
}
