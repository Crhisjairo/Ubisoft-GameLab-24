using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace _Scripts.UI
{
    public class FinalScreen : MonoBehaviour
    {
        public float transitionSpeed;
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        [HideInInspector] public UnityEvent<bool> OnFinishFadeIn;
        
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        
        public void FadeIn()
        {
            StartCoroutine(StartFadeIn());
        }
        
        
        IEnumerator StartFadeIn()
        {
            float alpha = _canvasGroup.alpha;
            _canvas.enabled = true;

            while (alpha < 1)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                alpha += 0.01f * transitionSpeed;
                _canvasGroup.alpha = alpha;
            }
            
            OnFinishFadeIn?.Invoke(true);
        }
    }
}
