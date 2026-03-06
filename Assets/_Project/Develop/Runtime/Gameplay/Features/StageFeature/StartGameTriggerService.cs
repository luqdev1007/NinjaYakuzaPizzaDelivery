namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class StartGameTriggerService
    {
        public bool IsStartRequested { get; private set; }

        public void RequestStart() => IsStartRequested = true;
        public void Reset() => IsStartRequested = false;
    }
}