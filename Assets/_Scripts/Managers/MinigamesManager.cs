using Assets._Scripts.Shared;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace Assets._Scripts.Managers
{
    public class MinigamesManager : MonoBehaviour
    {
        [SerializeField] private MinigameSceneNames defaultScene;

        [SerializeField] private Animator spinnerAnimator;
        [SerializeField] private Animator transitionCanvasAnim;

        [SerializeField] private float waitTimeBeforeLoading;
        [SerializeField] private float waitTimeAfterLoading;

        [SerializeField] private bool saveOnLoad = false;
        [SerializeField] private float waitTimeForSaving = 5f;

        [SerializeField] private Slider loadingSlider;
        //TODO: Trigger the save data.

        private Animator _loadingScreenAnimator;
        private Coroutine _loadingScreenRoutine;

        private int scenesCount;

        private void Awake()
        {
            SetComponents();
            scenesCount = Enum.GetNames(typeof(MinigameSceneNames)).Length;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            StartLoadScreenAnimations(TransitionAnimations.FadeIn);
        }

        private void SetComponents()
        {
            _loadingScreenAnimator = GetComponent<Animator>();
        }

        public void StartLoadScreen(string sceneToLoad)
        {
            if (_loadingScreenRoutine is null)
            {
                _loadingScreenRoutine = StartCoroutine(LoadScreenAsync(sceneToLoad));
            }
        }

        private IEnumerator LoadScreenAsync(string sceneToLoad, Action onRoutineStarts = null)
        {
            StartLoadScreenAnimations(TransitionAnimations.FadeIn);

            yield return new WaitForSecondsRealtime(waitTimeBeforeLoading);

            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad);
            loading.completed += OnCompleteLoading;

            while (!loading.isDone)
            {
                loadingSlider.value = loading.progress;

                yield return null;
            }

            yield return new WaitForSecondsRealtime(waitTimeAfterLoading);

            FinishLoadScreenAnimations();
            onRoutineStarts?.Invoke();

            yield return new WaitForSecondsRealtime(waitTimeForSaving);

            Destroy(gameObject);
        }
        private void StartLoadScreenAnimations(TransitionAnimations transition)
        {
            spinnerAnimator.enabled = true;
            transitionCanvasAnim.enabled = true;

            _loadingScreenAnimator.Play(transition.ToString());
        }

        private void FinishLoadScreenAnimations()
        {
            spinnerAnimator.enabled = false;
            transitionCanvasAnim.enabled = false;
        }

        private void OnCompleteLoading(AsyncOperation loading)
        {
            _loadingScreenAnimator.Play(TransitionAnimations.FadeOut.ToString());
        }
    }
}