﻿using System;
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
        
        [Space(10)]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float maxHealth = 100f;
        
        [SyncVar]
        private float _currentHealth;
        
        [SyncVar(hook = nameof(OnChangeStateClient))]
        private BossStates _currentStateEnum;
        
        private IBossState _currentStateBehaviour;

        [HideInInspector] public Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
            
            ChangeBossStateOnServer(BossStates.BossState1);
        }

        #region Server

        [Command]
        public void CmdTakeDamage(float damage)
        {
            if (!isServer) return;
            
            _currentHealth -= damage;
            healthSlider.value = _currentHealth;
            
            if (_currentHealth <= 0)
            {
                // On death.
            }
        }
        
        public void ChangeBossStateOnServer(BossStates newState)
        {
            // Check if server needs to change state whit OnChangeStateClient.
            OnChangeStateClient(_currentStateEnum, newState);
            _currentStateEnum = newState;
        }

        #endregion

        #region Client

        public void TakeDamage(float damage)
        {
            CmdTakeDamage(damage);
        }
        
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

        #endregion
    }
}