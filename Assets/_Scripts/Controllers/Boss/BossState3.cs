using UnityEngine;

namespace _Scripts.Controllers.Boss
{
    public class BossState3 : MonoBehaviour, IBossState
    { 
        private Boss _boss;
       
        public void StartState(Boss boss)
        {
            _boss = boss;
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}