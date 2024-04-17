using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Controllers.Boss
{
    public class BossState3 : MonoBehaviour, IBossState
    { 
        private Boss _boss;
        
        public List<Transform> waypoints;
        public float attackWaitTime = 10f;
        
        public float baseSpeed = 20f;
        
        public float particleSpeed = 1.5f;
        public float startSpeed = 35f;
        
        private int currentWaypointIndex = 0;
        Coroutine _attackRoutine;
       
        public void StartState(Boss boss)
        {
            _boss = boss;
            
            var main = boss.ParticleSystem.main;
            main.simulationSpeed = particleSpeed;
            main.startColor = Color.red;
            main.startSpeed = startSpeed;
            
            MoveToNextWaypoint();
            _attackRoutine = StartCoroutine(AttackRoutine());
        }
        public void StopState()
        {
            LeanTween.cancel(_boss.gameObject);
            StopCoroutine(_attackRoutine);
        }
        
        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(attackWaitTime);
                
                if(_boss._animator.GetBool("Attacked")) continue;
                
                _boss._animator.SetTrigger("OnAttack");
                yield return new WaitForSeconds(0.3f);
                // Throw a missile to the player
                _boss.SpawnMissile();
            }
            
        }

        void MoveToNextWaypoint()
        {
            float distance = Vector2.Distance(_boss.transform.position, waypoints[currentWaypointIndex].position);
            float duration = distance / baseSpeed;

            LeanTween.move(_boss.gameObject, waypoints[currentWaypointIndex].position, duration)
                .setEase(LeanTweenType.easeInOutElastic)
                .setOnComplete(OnWaypointReached);
        }

        void OnWaypointReached()
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0;
            }
            MoveToNextWaypoint();
        }
    }
}