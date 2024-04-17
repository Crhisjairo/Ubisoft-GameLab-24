using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Controllers.Boss
{
    public class BossState2 : MonoBehaviour, IBossState
    {
        private Boss _boss;

        public List<Transform> waypoints;
        public float baseSpeed = 20f;
        public float aceleleration = 1.5f;
        public float attackWaitTime = 5f;
        
        public float particleSpeed = 2f;
        public float startSpeed = 15f;
        
        private int currentWaypointIndex = 0;

        private bool _isAccelerating = false;
        
        Coroutine _attackRoutine;
        
        public void StartState(Boss boss)
        {
            _boss = boss;
            var main = boss.ParticleSystem.main;
            main.simulationSpeed = particleSpeed;
            main.startColor = Color.magenta;
            main.startSpeed = startSpeed;
            
            MoveToNextWaypoint();
            _attackRoutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(attackWaitTime);
                
                if(_boss._animator.GetBool("Attacked")) continue;
                
                _boss._animator.SetTrigger("OnAttack");
                _boss.CmdSendTriggerAnimation("OnAttack");
                yield return new WaitForSeconds(0.3f);
                // Throw a missile to the player
                _boss.SpawnMissile();
            }
            
        }

        public void StopState()
        {
            Debug.Log("Stop BossState2");
            LeanTween.cancel(_boss.gameObject);
            StopCoroutine(_attackRoutine);
        }

        void MoveToNextWaypoint()
        {
            float distance = Vector2.Distance(_boss.transform.position, waypoints[currentWaypointIndex].position);
            float duration = distance / baseSpeed;

            LeanTween.move(_boss.gameObject, waypoints[currentWaypointIndex].position, duration)
                .setEase(LeanTweenType.easeInQuad)
                .setOnComplete(OnWaypointReached);
        }

        void OnWaypointReached()
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0;
            }
            if (!_isAccelerating)
            {
                baseSpeed *= aceleleration;
                _isAccelerating = true;
            }
            else
            {
                baseSpeed /= aceleleration;
                _isAccelerating = false;
            }
            
            
            MoveToNextWaypoint();
        }
    }
}