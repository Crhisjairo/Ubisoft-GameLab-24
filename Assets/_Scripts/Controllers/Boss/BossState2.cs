using UnityEngine;

namespace _Scripts.Controllers.Boss
{
    public class BossState2 : MonoBehaviour, IBossState
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