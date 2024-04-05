using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Map
{
    [RequireComponent(typeof(Light2D))]
    public class FlashingLight : MonoBehaviour
    {
        [SerializeField] private float flashSpeed = 0.1f;
        [SerializeField] private float minIntensity = 0.6f, maxIntensity = 0.8f;

        public bool changeSmooth;
        
        private Light2D _light2D;

        public virtual void Awake()
        {
            SetComponents();
        }

        public virtual void Start()
        {
            StartCoroutine(StartIncandescentFlashing());
        }
        
        private void SetComponents()
        {
            _light2D = GetComponent<Light2D>();
        }
        
        
        private IEnumerator StartIncandescentFlashing()
        {
            while (true)
            {
               ChangeLightIntensityRandom();
               yield return new WaitForSecondsRealtime(flashSpeed);
            }
        }

        private void ChangeLightIntensityRandom()
        {
            var newIntensity = Random.Range(minIntensity, maxIntensity);
            
            if (changeSmooth)
            {
                var startIntensity = _light2D.intensity;

                LeanTween.value(gameObject,
                    (intensityValue) =>
                    {
                        _light2D.intensity = intensityValue;
                    }, 
                    startIntensity, newIntensity, flashSpeed / 2);
            }
            else
            {
                _light2D.intensity = newIntensity;
            }
        }
    }
}