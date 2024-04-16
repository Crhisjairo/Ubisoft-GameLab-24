
namespace _Scripts.Controllers.Bombs
{
    public class StrongThrowableBomb : ThrowableBomb
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
