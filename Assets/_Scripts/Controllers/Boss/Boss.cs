using System;
using _Scripts.Controllers.Bombs;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Controllers.Boss
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Boss : NetworkBehaviour
    {
        public BossState1 BossState1;
        public BossState2 BossState2;
        public BossState3 BossState3;
        
        public int bossState1Limit = 85;
        public int bossState2Limit = 50;
        public int bossState3Limit = 35;

        [Space(10)]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float maxHealth = 100f;
        
        [SyncVar(hook = nameof(OnCurrentHealthChanged))]
        private float _currentHealth;
        
        [SyncVar(hook = nameof(OnChangeStateClient))]
        private BossStates _currentStateEnum;
        
        private IBossState _currentStateBehaviour;

        [HideInInspector] public Rigidbody2D rb;

        private Animator _animator;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
            
            ChangeBossStateOnServer(BossStates.BossState1);
        }

        #region Server

        [Server]
        public void TakeDamage(float damage)
        {
            if (!isServer) return;
            
            _currentHealth -= damage;
            healthSlider.value = _currentHealth;
            
            _animator.SetTrigger("Attacked");
            RpcReceiveBoolAnimation("Attacked", true);
            
            if(_currentHealth <= bossState1Limit && _currentStateEnum == BossStates.BossState1)
                ChangeBossStateOnServer(BossStates.BossState2);
            else if(_currentHealth <= bossState2Limit && _currentStateEnum == BossStates.BossState2)
                ChangeBossStateOnServer(BossStates.BossState3);
            
            if (_currentHealth <= 0)
            {
                // On death.
            }
        }
        
        public void ChangeBossStateOnServer(BossStates newState)
        {
            // Check if server needs to change state whit OnChangeStateClient.
            OnChangeStateClient(_currentStateEnum, newState);
            //_currentStateEnum = newState;
        }
        
        public float pushForce = 10f;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if(!isServer) return;
            
            if (other.collider.CompareTag("Player"))
            {
                Rigidbody rb = other.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                    rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                }
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



        #endregion

        #region Client

        public void OnChangeStateClient(BossStates oldValue, BossStates newValue)
        {
            _currentStateEnum = newValue;

            switch (newValue)
            {
                case BossStates.BossState1:
                    _currentStateBehaviour = BossState1;
                    _currentStateBehaviour.StartState(this);
                    break;
                case BossStates.BossState2:
                    _currentStateBehaviour = BossState2;
                    _currentStateBehaviour.StartState(this);
                    break;
                case BossStates.BossState3:
                    _currentStateBehaviour = BossState3;
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
        
        [ClientRpc]
        private void RpcReceiveBoolAnimation(string animationName, bool value)
        {
            _animator.SetBool(animationName, value);
        }
       

        #endregion
    }
}