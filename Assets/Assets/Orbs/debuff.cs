using _Scripts.Controllers;
using Mirror;
using UnityEngine;
namespace Assets.Orbs
{
    public class Debuff : Orb
    {
        protected override void Start()
        {
            amountText.text = (-orbAmount).ToString();
        }

        
        public override void OnApplyOrbEffect(PlayerController playerController)
        {
            playerController.RemoveStrongBombs(orbAmount);
           
        }
    }
}
