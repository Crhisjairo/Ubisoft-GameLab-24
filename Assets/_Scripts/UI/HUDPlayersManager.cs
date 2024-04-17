using System;
using _Scripts.UI.MainMenu;
using _Scripts.UI.PlayerUIs;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class HUDPlayersManager : MonoBehaviour
    {
        public PlayerUI player1UI;
        public PlayerUI player2UI;

        public static HUDPlayersManager Instance { get; private set; }
        
        [SerializeField] private TutorialBox tutorialBox;

        [SerializeField] private TextMeshProUGUI timerText;
        private float _remainingTime;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            UpdateCounterTimer();
        }
        
        public void SetInitialTimer(float time)
        {
            _remainingTime = time;
        }

        public void StartTutorialAnimation()
        {
            tutorialBox.StartAnimation();
        }
        
        private void UpdateCounterTimer()
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                int mins, secs, milisecs;
                
                
                if (_remainingTime <= 0)
                {
                    mins = Mathf.FloorToInt(0);
                    secs = Mathf.FloorToInt(0);
                    milisecs = Mathf.FloorToInt(0);
                }
                else
                {
                    mins = Mathf.FloorToInt(_remainingTime / 60);
                    secs = Mathf.FloorToInt(_remainingTime % 60);
                    milisecs = Mathf.FloorToInt((_remainingTime * 1000) % 1000);
                }

                timerText.SetText(
                    string.Format(
                        "{0:D2}:{1:D2}:{2,2}", 
                        mins, 
                        secs, 
                        milisecs.ToString("D2")[..2])
                    );
            }
        }
    }
}
