namespace _Scripts.Controllers.Boss
{
    public interface IBossState
    {
        
        /// <summary>
        /// Must be called before any other method.
        /// </summary>
        /// <param name="boss"></param>
        public void StartState(Boss boss);

        public void StopState();
    }
}