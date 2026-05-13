using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers
{
    public class InGameTimerFeatureService
    {
        public ReactiveVariable<float> ElapsedTime { get; } = new(0);
        private bool _isActive;

        public void Start() => _isActive = true;
        public void Stop() => _isActive = false;
        public void Reset() => ElapsedTime.Value = 0;

        public void Tick(float deltaTime)
        {
            if (_isActive)
                ElapsedTime.Value += deltaTime;
        }
    }
}