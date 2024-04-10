using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using Mirror;
using UnityEngine;
namespace Assets.Orbs
{
    public class OrbStrong : Orb
    {
        [SyncVar]
        private bool _isFirstSend = true;
        

        public override void OnApplyOrbEffect(PlayerController playerController)
        {
           // if(_isFirstSend)
               // GlobalPlayerData.Instance.CmdAddStrongBomb(orbAmount);
            if(_isFirstSend)
            {
                playerController.AddStrongBombs(orbAmount);
            }

            _isFirstSend = false;
        }
    }
}