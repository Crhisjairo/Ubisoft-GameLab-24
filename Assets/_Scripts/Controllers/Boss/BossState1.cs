
using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Controllers.Boss
{
    public class BossState1 : MonoBehaviour, IBossState
    {
        public Transform[] waypoints;
        public float speed = 5f;
        private int currentWaypointIndex = 0;
        
        private Boss _boss;
       
        public void StartState(Boss boss)
        {
            _boss = boss;
            MoveToNextWaypoint();
        }

        void MoveToNextWaypoint()
        {
            float distance = Vector2.Distance(_boss.transform.position, waypoints[currentWaypointIndex].position);
            float duration = distance / speed;

            LeanTween.move(_boss.gameObject, waypoints[currentWaypointIndex].position, duration)
                .setEase(LeanTweenType.easeInQuad)
                .setOnComplete(OnWaypointReached);
        }

        void OnWaypointReached()
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
            MoveToNextWaypoint();
        }
    }
}