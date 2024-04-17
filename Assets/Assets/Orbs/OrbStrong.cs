using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using Mirror;
using UnityEngine;
namespace Assets.Orbs
{
    public class OrbStrong : Orb
    {
        public override void OnApplyOrbEffect(PlayerController playerController)
        {
            playerController.AddStrongBombs(orbAmount);
            
        }
    }
}