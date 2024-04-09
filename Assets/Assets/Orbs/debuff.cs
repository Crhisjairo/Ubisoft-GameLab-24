using _Scripts.Controllers;
namespace Assets.Orbs
{
    public class Debuff : Orb
    {
        // Override the Awake method to set a different gravity scale
        protected override void Awake()
        {
            gravityScale = 0.22f;
            base.Awake();
        }

        public override void OnApplyOrbEffect(PlayerController playerController)
        {
            
        }
    }
}
