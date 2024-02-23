using System;
using System.Collections;
using System.Diagnostics;
using Assets._Scripts.Shared;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace _Scripts.Managers
{
    public class MinigamesManager : MonoBehaviour
    {
        [SerializeField] private MinigameSceneNames defaultScene;

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
        [FormerlySerializedAs("cam")]
        [SerializeField] private CinemachineVirtualCamera vCam;

        private Coroutine _loadingScreenRoutine;

        private int scenesCount;
        private bool _isLoading;
        
        private float _loadProgress = 1f;
        private float _loadBarsmoothness = 0.1f;

        private AsyncOperation loading;
        
        public float tiempoInicial; // Tiempo inicial en segundos
        private float tiempoRestante; // Tiempo actual restante


        private void Awake()
        {
            SetupComponents();
            
            scenesCount = Enum.GetNames(typeof(MinigameSceneNames)).Length;
        }
        
        private void SetupComponents()
        {
            uiCanvasGroup.alpha = 0;
            spinnerAnimator.enabled = false;
            transitionCanvasAnim.enabled = false;

            tiempoRestante = tiempoInicial = timeForEachScene;
        }

        private void Start()
        {
            StartLoadScreen(defaultScene.ToString());
        }

        public void StartLoadScreen(string sceneToLoad)
        { 
            _loadingScreenRoutine = StartCoroutine(LoadScreenAsync(sceneToLoad));
        }

        private void Update()
        {
            if (_isLoading)
            {
                UpdateLoadingBar();
            }
            else
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
                _loadBarsmoothness = 2f;
            }
                
            float target = _loadProgress;
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, target, _loadBarsmoothness * Time.deltaTime);
        }

        private void UpdateCounterTimer()
        {
            // Verifica que el tiempo no sea negativo para evitar mostrar números negativos
            if (tiempoRestante > 0)
            {
                // Decrementa el tiempo restante en cada fotograma
                tiempoRestante -= Time.deltaTime;

                int minutos = Mathf.FloorToInt(tiempoRestante / 60);
                int segundos = Mathf.FloorToInt(tiempoRestante % 60);
                int milisegundos = Mathf.FloorToInt((tiempoRestante * 1000) % 1000);
                
                // Actualiza el texto del contador en la interfaz de usuario
                timerText.SetText(
                    string.Format("{0:D2}:{1:D2}:{2:D2}", minutos, segundos, milisegundos)
                );
            }
            else
            {
                // Puedes realizar acciones cuando el tiempo llega a cero
                Debug.Log("¡Tiempo agotado!");
            }
        }

        private IEnumerator LoadScreenAsync(string sceneToLoad)
        {
            _isLoading = true;
         
            SetActiveAnimators(true);
            transitionCanvasAnim.Play(TransitionAnimations.FadeIn.ToString());

            yield return new WaitForSecondsRealtime(startAnimTime);
            
            loading = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            
            while (!loading.isDone)
            {
                _loadProgress = loading.progress;
                Debug.Log(_loadProgress);
                
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(waitTimeAfterLoading);
            // When the scene is loaded, fade out the transition canvas.
            transitionCanvasAnim.Play(TransitionAnimations.FadeOut.ToString());
            //yield return null;

            yield return new WaitForSecondsRealtime(endAnimTime);
            
            _isLoading = false;

            //brain.gameObject.SetActive(false);
            vCam.gameObject.SetActive(false);
            
            SetActiveAnimators(false);
            
            //yield return null;
        }
        
        private void SetActiveAnimators(bool isActive)
        {
            spinnerAnimator.enabled = isActive;
            transitionCanvasAnim.enabled = isActive;
        }

        private void OnCompleteLoading(AsyncOperation loading)
        {
            transitionCanvasAnim.Play(TransitionAnimations.FadeOut.ToString());
        }
    }
}