using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using UnityEngine;
namespace Assets.Orbs
{
    public class NormalOrb : Orb
    {
        private bool _isFirstSend = true;
        
        public override void OnApplyOrbEffect(PlayerController playerController)
        {
            // if(_isFirstSend)
            //     GlobalPlayerData.Instance.AddRegularBomb(orbAmount);
            
            // _isFirstSend = false;
        }
    }
}