namespace _Scripts.Controllers.PlayerMoveSets
{
    public class RedBomb : Bomb
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            speed = 7f;
            gravityScale = 0.02f;
            base.Start();
        }

    }
}
