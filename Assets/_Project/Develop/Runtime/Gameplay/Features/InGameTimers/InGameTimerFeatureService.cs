using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers
{
    public class InGameTimerFeatureService
    {
        public event Action OnTimerShowRequested;
        public event Action OnTimerHideRequested;

        private float _elapsedTime;
        private bool _isActive;

        // То самое свойство, которое ищет WinPopupPresenter
        public float ElapsedTime => _elapsedTime;

        public void Show()
        {
            _elapsedTime = 0; // Сбрасываем при старте уровня
            _isActive = true;
            OnTimerShowRequested?.Invoke();
        }

        public void Hide()
        {
            _isActive = false;
            OnTimerHideRequested?.Invoke();
        }

        // Этот метод будем вызывать из стейта
        public void Update(float deltaTime)
        {
            if (_isActive)
            {
                _elapsedTime += deltaTime;
            }
        }
    }
}