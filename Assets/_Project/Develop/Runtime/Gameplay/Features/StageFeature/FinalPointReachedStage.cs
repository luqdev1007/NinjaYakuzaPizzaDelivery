using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class FinalPointReachedStage : IStage
    {
        private readonly FinalPointTriggerService _finalPointTrigger;
        private readonly LevelProgressService _levelProgressService;
        private readonly MainHeroHolderService _heroHolder;
        private readonly Vector3 _finalPointPosition;
        private readonly ReactiveEvent _completed = new();

        private bool _inProcess;
        private bool _isProgressInitialized;

        public IReadOnlyEvent Completed => _completed;

        public FinalPointReachedStage(
            FinalPointTriggerService finalPointTrigger,
            LevelProgressService levelProgressService,
            MainHeroHolderService heroHolder,
            Vector3 finalPointPosition)
        {
            _finalPointTrigger = finalPointTrigger;
            _levelProgressService = levelProgressService;
            _heroHolder = heroHolder;
            _finalPointPosition = finalPointPosition;
        }

        public void Start()
        {
            // 1. Создаем точку финиша (теперь это безопасно делать в PrepState)
            _finalPointTrigger.Create(_finalPointPosition);

            // 2. Флаг работы стейджа
            _inProcess = true;

            // Попытка инициализации прогресса (сработает, только если герой уже заспавнен)
            TryInitializeProgress();
        }

        public void Update(float deltaTime)
        {
            if (_inProcess == false)
                return;

            // Если прогресс еще не был инициализирован (герой появился позже), пробуем снова
            if (_isProgressInitialized == false)
                TryInitializeProgress();

            _finalPointTrigger.Update(deltaTime);

            if (_finalPointTrigger.HasMainHeroContact.Value)
                ProcessEnd();
        }

        private void TryInitializeProgress()
        {
            // Проверяем наличие героя и его транформа
            if (_heroHolder.MainHero != null && _heroHolder.MainHero.Transform != null)
            {
                _levelProgressService.Initialize(_heroHolder.MainHero.Transform.position);
                _isProgressInitialized = true;
            }
        }

        public void Cleanup()
        {
            _levelProgressService.Reset();
            _finalPointTrigger.Cleanup();
            _inProcess = false;
            _isProgressInitialized = false;
        }

        public void Dispose() => Cleanup();

        private void ProcessEnd()
        {
            _inProcess = false;
            _completed.Invoke();
        }
    }
}