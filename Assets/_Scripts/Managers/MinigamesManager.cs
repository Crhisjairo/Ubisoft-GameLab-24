using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _Scripts.Shared;
using Assets._Scripts.Shared;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace _Scripts.Managers
{
    public class MinigamesManager : MonoBehaviour
    {
        [FormerlySerializedAs("sceneLoader")]
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private MinigameSceneNames[] scenesNamesToLoad;
        private Queue<MinigameSceneNames> _scenesSortedRan;
        
        [Tooltip("Time in seconds for each scene to be played.")]
        [SerializeField] private float timeForEachScene = 60f;
        [Space(20)]
        [SerializeField] private CanvasGroup uiCanvasGroup;
        [SerializeField] private Animator transitionCanvasAnim;
        [SerializeField] private Animator spinnerAnimator;
        
        [SerializeField] private TextMeshProUGUI timerText;
        
        [Space(20)]
        [SerializeField] private float startAnimTime = 3;
        [SerializeField] private float waitTimeAfterLoading = 0.3f;
        [SerializeField] private float endAnimTime = 0.3f;

        [Space(20)]
        [SerializeField] private Slider loadingSlider;
        //TODO: Trigger the save data.

        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineVirtualCamera vCam;

        private Coroutine _loadingScreenRoutine;
        private MinigameSceneNames _currentMinigameScene = MinigameSceneNames.Empty;

        private bool _isLoading;
        
        private float _loadProgress = 1f;
        private float _loadBarsmoothness = 0.1f;

        private AsyncOperation loading;

        private bool _isTimerOn = true;
        private float _initialTime;
        private float _remainingTime;
        
        private void Awake()
        {
            SetupComponents();
        }

        private void SetupComponents()
        {
            _scenesSortedRan = new Queue<MinigameSceneNames>(0);
            
            uiCanvasGroup.alpha = 0;
            spinnerAnimator.enabled = false;
            transitionCanvasAnim.enabled = false;
            
            ResetTimer();
            ShuffleScenesToLoad();
        }

        private void ShuffleScenesToLoad()
        {
            Debug.Log("New order for scenes: ");
            int position = 0;
            
            // shuffle the scenes to load.
            foreach (var scene in scenesNamesToLoad.OrderBy(x => Random.value))
            {
                _scenesSortedRan.Enqueue(scene);
                Debug.Log("Scene: " + scene + " -> in position: " + position);
                position++;
            }
            
        }
        

        private void Start()
        {
            StartLoadScreen(_scenesSortedRan.Dequeue().ToString()); // TODO: using the first scence name to debug.
        }

        public void StartLoadScreen(string sceneToLoad)
        {
            _loadingScreenRoutine = StartCoroutine(
                LoadNextSceneAsync(sceneToLoad)
                );
        }

        private void Update()
        {
            if (_isLoading)
            {
                UpdateLoadingBar();
            }
            else if(_isTimerOn)
            {
                // TODO: Start counter timer more precisely. Start after the loading screen is done.
                UpdateCounterTimer();    
            }
            
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

        private void UpdateCounterTimer()
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;

                int mins = Mathf.FloorToInt(_remainingTime / 60);
                int secs = Mathf.FloorToInt(_remainingTime % 60);
                int milisecs = Mathf.FloorToInt((_remainingTime * 1000) % 1000);
                
                timerText.SetText(
                    string.Format("{0:D2}:{1:D2}:{2:D2}", mins, secs, milisecs)
                );
            }
            else
            {
                LoadNextMinigame();
            }
        }

        private void LoadNextMinigame()
        {
            ResetLoadingBar();
            ResetTimer();
            
            if(_scenesSortedRan.Count == 0)
            {
                //TODO: save data, unload MinigamesManagerScene and load the boss battle scene.
                LoadBossBattleScene();
            }
            else
            {
                StartLoadScreen(_scenesSortedRan.Dequeue().ToString());
            }
        }

        private void LoadBossBattleScene()
        {
            _isTimerOn = false;
            // Call another script for loading the boss battle scene.
            Debug.Log("Loading boss battle scene!.");

            _sceneLoader.LoadNextScene();
            //StartCoroutine(UnloadSceneAsync(_currentMinigameScene.ToString()));
            
        }

        private IEnumerator LoadNextSceneAsync(string sceneToLoad)
        {
            _isLoading = true;
            SetActiveAnimators(true);
            transitionCanvasAnim.Play(TransitionAnimations.FadeIn.ToString());

            yield return new WaitForSecondsRealtime(startAnimTime);
            
            // unload the current scene if it's not null.
            if (_currentMinigameScene != MinigameSceneNames.Empty)
            {
                // This will render frames until the scene is unloaded.
                yield return UnloadSceneAsync(_currentMinigameScene.ToString());
            }
            
            loading = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            
            while (!loading.isDone)
            {
                _loadProgress = loading.progress;
                // Render frames while loading the scene.
                yield return null;
            }
            
            _currentMinigameScene = (MinigameSceneNames) Enum.Parse(typeof(MinigameSceneNames), sceneToLoad);
            
            yield return new WaitForSecondsRealtime(waitTimeAfterLoading);
            // When the scene is loaded, fade out the transition canvas.
            transitionCanvasAnim.Play(TransitionAnimations.FadeOut.ToString());

            yield return new WaitForSecondsRealtime(endAnimTime);
            
            _isLoading = false;
            loading = null;
            //brain.gameObject.SetActive(false);
            vCam.gameObject.SetActive(false);
            SetActiveAnimators(false);
        }
        
        private IEnumerator UnloadSceneAsync(string sceneToUnload)
        {
            AsyncOperation unloading = SceneManager.UnloadSceneAsync(sceneToUnload);
            
            while (!unloading.isDone)
            {
                // Render frames while unloading the scene.
                yield return null;
            }
        }
        
        private void SetActiveAnimators(bool isActive)
        {
            spinnerAnimator.enabled = isActive;
            transitionCanvasAnim.enabled = isActive;
        }

        private void ResetLoadingBar()
        {
            // Reseting loading bar.
            _loadProgress = 1f;
            _loadBarsmoothness = 0.1f;
            loadingSlider.value = 0;
        }
        
        private void ResetTimer()
        {
            _remainingTime = _initialTime = timeForEachScene;
            int mins = Mathf.FloorToInt(_remainingTime / 60);
            int secs = Mathf.FloorToInt(_remainingTime % 60);
            int milisecs = Mathf.FloorToInt((_remainingTime * 1000) % 1000);
            
            timerText.SetText(
                string.Format("{0:D2}:{1:D2}:{2:D2}", mins, secs, milisecs)
                );
        }
    }
}