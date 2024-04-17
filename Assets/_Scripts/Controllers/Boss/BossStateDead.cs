using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Controllers.Boss
{
    public class BossStateDead : MonoBehaviour, IBossState
    {
        public Transform endPoint;
        public float speed = 10f;
        
        public float particleSpeed = 10f;
        public float startSpeed = 100f;
        
        public void StartState(Boss boss)
        {
            var main = boss.ParticleSystem.main;
            main.startColor = Color.white;
            main.startSpeed = startSpeed;
            main.simulationSpeed = particleSpeed;
            
            LeanTween.move(boss.gameObject, endPoint, speed).setOnComplete(() =>
            {
                boss.rb.isKinematic = false;
                boss.rb.bodyType =  RigidbodyType2D.Dynamic;
                boss.rb.gravityScale = 1;

                boss.ParticleSystem.Stop();
                
                boss._animator.SetTrigger("OnDead");
                boss.CmdSendTriggerAnimation("OnDead");
            });
            
            boss.finalScreen.OnFinishFadeIn.AddListener(boss.winSection.SetActive);
            boss.finalScreen.FadeIn();
            HUDPlayersManager.Instance.gameObject.SetActive(false);

        }
        public void StopState()
        {
            throw new System.NotImplementedException();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}