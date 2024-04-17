using System;
using System.Collections;
using _Scripts.Controllers.Bombs;
using _Scripts.UI;
using _Scripts.UI.MainMenu;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.Controllers.Boss
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Boss : NetworkBehaviour
    {
        public BossState1 BossState1;
        public BossState2 BossState2;
        public BossState3 BossState3;
        public BossStateDead BossStateDead;
        
        public int bossState1Limit = 75;
        public int bossState2Limit = 45;
        public int bossState3Limit = 35;

        public Transform handPosition;
        public GameObject missilePrefab;

        public ParticleSystem ParticleSystem;
        
        public FinalScreen finalScreen;

        public MenuSection winSection, loseSection;

        [Space(10)]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float maxHealth = 100f;
        
        [SyncVar(hook = nameof(OnCurrentHealthChanged))]
        private float _currentHealth;
        
        [SyncVar(hook = nameof(OnChangeStateClient))]
        private BossStates _currentStateEnum;
        
        private IBossState _currentStateBehaviour;

        [HideInInspector] public Rigidbody2D rb;

        [HideInInspector] public Animator _animator;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private PlayerController[] players = new PlayerController[2];
        
        private void Start()
        {
            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
            
            ChangeBossStateOnServer(BossStates.BossState1);

            //if(!isServer) return;
            

            //StartCoroutine(CheckIfPlayerDead());
        }

        IEnumerator CheckIfPlayerDead()
        {
            bool gameOver = false;
            
            players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            Debug.Log("Player 1 on Server with: " + HUDPlayersManager.Instance.player1UI.lifeUIController.hearts);
            Debug.Log("Player 2 on Server with:" + HUDPlayersManager.Instance.player2UI.lifeUIController.hearts);

            yield return new WaitForSeconds(5f);
            
            while (!gameOver)
            {
                gameOver = players[0].playerServerDataSync.GetHealth() <= 0 && players[1].playerServerDataSync.GetHealth() <= 0;
                
                yield return null;
            }

            finalScreen.OnFinishFadeIn.AddListener(loseSection.SetActive);
            finalScreen.FadeIn();
        }
        

        
        private void Update()
        {
            if(!isServer) return;
            
            
            
        }

        #region Server

        //[Server]
        public void TakeDamage(float damage)
        {
            //if (!isServer) return;
            
            _currentHealth -= damage;
            healthSlider.value = _currentHealth;
            
            _animator.SetTrigger("Attacked");
            CmdSendTriggerAnimation("Attacked");
            //RpcReceiveBoolAnimation("Attacked", true);
            
            
            
            if(_currentHealth <= bossState1Limit && _currentStateEnum == BossStates.BossState1)
                ChangeBossStateOnServer(BossStates.BossState2);
            else if(_currentHealth <= bossState2Limit && _currentStateEnum == BossStates.BossState2)
                ChangeBossStateOnServer(BossStates.BossState3);
            else if(_currentHealth <= bossState3Limit && _currentStateEnum == BossStates.BossState3)
                ChangeBossStateOnServer(BossStates.BossStateDead);
            
        }
        
        public void ChangeBossStateOnServer(BossStates newState)
        {
            // Check if server needs to change state whit OnChangeStateClient.
            OnChangeStateClient(_currentStateEnum, newState);
            //_currentStateEnum = newState;
        }
        
        public float pushForce = 100f;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                PlayerController playerController = other.collider.GetComponent<PlayerController>();
                
                Vector2 pushDirection = (other.transform.position - transform.position).normalized;
                pushDirection *= pushForce;
                
                playerController.TakeDamage(pushDirection, 1);
                
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("ThrowableBomb"))
            {
                TakeDamage(
                    other.GetComponent<ThrowableBomb>().damage
                    );
            }
        }

        public void SpawnMissile()
        {
            GameObject missile = Instantiate(missilePrefab, handPosition.position, Quaternion.identity);
            Transform target = FindObjectsByType<PlayerController>(FindObjectsSortMode.None)[Random.Range(0, 2)].transform;
            Debug.Log("Missile target: " + target.name);
            
            missile.GetComponent<Missile>().SetTarget(target);
            
            NetworkServer.Spawn(missile);
        }
        
        #endregion

        #region Client

        public void OnChangeStateClient(BossStates oldValue, BossStates newValue)
        {
            _currentStateEnum = newValue;

            switch (newValue)
            {
                case BossStates.BossState1:
                    if(_currentStateBehaviour != null)
                        _currentStateBehaviour.StopState();
                    
                    _currentStateBehaviour = BossState1;
                    _currentStateBehaviour.StartState(this);
                    break;
                case BossStates.BossState2:
                    _currentStateBehaviour.StopState();
                    
                    _currentStateBehaviour = BossState2;
                    _currentStateBehaviour.StartState(this);
                    break;
                case BossStates.BossState3:
                    _currentStateBehaviour.StopState();
                    
                    _currentStateBehaviour = BossState3;
                    _currentStateBehaviour.StartState(this);
                    break;
                case BossStates.BossStateDead:
                    _currentStateBehaviour.StopState();

                    _currentStateBehaviour = BossStateDead;
                    _currentStateBehaviour.StartState(this);
                    break;
                default:
                    Debug.LogWarning("Invalid state on boss.");
                    break;
            }
            
            // Change state logic.
        }

        public void OnCurrentHealthChanged(float oldValue, float newValue)
        {
            _currentHealth = newValue;
            healthSlider.value = _currentHealth;
        }
        
        [Command]
        public void CmdSendTriggerAnimation(string animationName)
        {
            RpcReceiveTriggerAnimation(animationName);
        }
        
        [ClientRpc]
        private void RpcReceiveTriggerAnimation(string animationName)
        {
            _animator.SetTrigger(animationName);
        }
       

        #endregion
    }
}