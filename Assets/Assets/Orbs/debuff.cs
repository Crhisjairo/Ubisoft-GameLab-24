using _Scripts.Controllers;
using Mirror;
using UnityEngine;
namespace Assets.Orbs
{
    public class Debuff : Orb
    {
        [SyncVar]
        private bool _isFirstSend = true;
        
        protected override void Start()
        {
            amountText.text = (-orbAmount).ToString();
        }

        
        public override void OnApplyOrbEffect(PlayerController playerController)
        {
            if(_isFirstSend)
            {
                playerController.RemoveStrongBombs(orbAmount);
            }
            
            _isFirstSend = false;
        }
    }
}
