using System;
using UnityEngine;
namespace _Scripts.Map
{
    public class Cloud: MonoBehaviour
    {
        [SerializeField] private Sprite[] clouds;

        private void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = clouds[UnityEngine.Random.Range(0, clouds.Length)];
        }

    }
}
