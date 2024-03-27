using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Shared;
using Assets._Scripts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.Managers
{
    public class CanvasGroupFade: MonoBehaviour
    {
        [SerializeField] private GlobalScenesNames nextSceneToLoad;

        [SerializeField] private bool useLoadingBar = true;
        [SerializeField] private GameObject LoadingBarGameObject;
        
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] private Animator transitionCanvasAnim;
        [SerializeField] private Animator spinnerAnimator;

        [Space(20)]
        [SerializeField] private Slider loadingSlider;

        public float transitionSpeed = 5f;
        
        private Coroutine _loadingScreenRoutine;
        private bool _isLoading;
        
        private float _loadProgress = 1f;
        private float _loadBarsmoothness = 0.1f;

        private AsyncOperation loading;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadingBarGameObject.SetActive(useLoadingBar);
        }

        private void Update()
        {
            if (_isLoading)
            {
                UpdateLoadingBar();
            }
        }

        public IEnumerator FadeIn()
        {
            float alpha = uiCanvasGroup.alpha;

            while (alpha < 1)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                alpha += 0.01f * transitionSpeed;
                uiCanvasGroup.alpha = alpha;
            }
        }
        
        public IEnumerator FadeOut()
        {
            float alpha = uiCanvasGroup.alpha;

            while (alpha > 0)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                alpha -= 0.01f * transitionSpeed;
                uiCanvasGroup.alpha = alpha;
            }
        }
        
        public void ShowScreenNoDelay()
        {
            uiCanvasGroup.alpha = 1f;
        }

        private void SetActiveAnimators(bool isActive)
        {
            spinnerAnimator.enabled = isActive;
            transitionCanvasAnim.enabled = isActive;
        }
        
        private void UpdateLoadingBar()
        {
            if (loading is not null)
            {
                _loadProgress = loading.progress;
                _loadBarsmoothness = 1f;
            }
                
            float target = _loadProgress;
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, target, _loadBarsmoothness * Time.deltaTime);
        }
    }
}
