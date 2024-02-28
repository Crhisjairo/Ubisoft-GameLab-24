using System;
using System.Collections;
using _Scripts.Shared;
using Assets._Scripts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.Managers
{
    public class SceneLoader: MonoBehaviour
    {
        [SerializeField] private GlobalScenesNames nextSceneToLoad;

        [SerializeField] private bool useLoadingBar = true;
        [SerializeField] private GameObject LoadingBarGameObject;
        
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] private Animator transitionCanvasAnim;
        [SerializeField] private Animator spinnerAnimator;

        [Space(20)]
        [SerializeField] private float startAnimTime = 3;
        [SerializeField] private float waitTimeBeforeDestroy = 0.3f;
        [SerializeField] private float endAnimTime = 0.3f;
        
        [Space(20)]
        [SerializeField] private Slider loadingSlider;
        
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

        public void LoadNextScene()
        {
            _loadingScreenRoutine = StartCoroutine(
                LoadNextSceneAsync(nextSceneToLoad.ToString())
                );
        }
        
        private IEnumerator LoadNextSceneAsync(string sceneToLoad)
        {
            _isLoading = true;
            SetActiveAnimators(true);
            
            transitionCanvasAnim.Play(TransitionAnimations.FadeIn.ToString());

            yield return new WaitForSecondsRealtime(startAnimTime);
            
            // This will render frames until the scene is unloaded.
            //AsyncOperation unloading = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            
            //while (!unloading.isDone)
           // {
                // Render frames while unloading the scene.
             //   yield return null;
            //}
            
            loading = SceneManager.LoadSceneAsync(sceneToLoad);
            
            while (!loading.isDone)
            {
                _loadProgress = loading.progress;
                // Render frames while loading the scene.
                yield return null;
            }
            
            // When the scene is loaded, fade out the transition canvas.
            transitionCanvasAnim.Play(TransitionAnimations.FadeOut.ToString());

            yield return new WaitForSecondsRealtime(endAnimTime);

            Destroy(gameObject, waitTimeBeforeDestroy);
            
            //_isLoading = false;
            //loading = null;
            
            //SetActiveAnimators(false);
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
