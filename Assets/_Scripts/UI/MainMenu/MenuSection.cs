using System;
using System.Collections;
using Assets._Scripts.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.MainMenu
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuSection : MonoBehaviour
    {
        private Animator animator;
        private CanvasGroup canvasGroup;

        [SerializeField] private Button firstSelectedButton;
        
        [SerializeField] private float transitionTime = 0.5f;
        public bool isActive = false;
        
        private bool _isTransitioning;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            gameObject.SetActive(isActive);
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.interactable = isActive;
        }

        public void SetActive(bool active)
        {
            isActive = active;
            
           if(isActive) gameObject.SetActive(isActive);
            
            canvasGroup.interactable = isActive;
            
            StartCoroutine(StartTransition());
        }

        public void SelectFirstButton()
        {
            firstSelectedButton.Select();
        }
        
        private IEnumerator StartTransition()
        {
            
            if (isActive) // On Open
            {
                ApplyCanvasTransition(TransitionAnimations.FadeIn);
                // TODO: Add others animations here. E.g. buttons that appear, etc.
            }
            else // On Close
            {
                ApplyCanvasTransition(TransitionAnimations.FadeOut);
                // TODO: Add others animations here. E.g. buttons that appear, etc.
            }

            if (!isActive)
            {
                yield return new WaitForSeconds(transitionTime);
                canvasGroup.interactable = isActive;
                gameObject.SetActive(isActive);
            }
                
        }

        
        private void ApplyCanvasTransition(TransitionAnimations animation)
        {
            // TODO: You can add more animations here. Maybe, animator controller must be changed.
            animator.Play(animation.ToString());
        }
    }
}