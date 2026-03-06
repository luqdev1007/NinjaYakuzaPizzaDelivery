using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class LevelProgressService
    {
        private readonly MainHeroHolderService _heroHolder;
        private readonly FinalPointTriggerService _finalPointTrigger;
        private readonly ReactiveVariable<float> _progress = new();

        public IReadOnlyVariable<float> Progress => _progress;

        private Vector3 _startPosition;
        private bool _isInitialized;

        public LevelProgressService(
            MainHeroHolderService heroHolder,
            FinalPointTriggerService finalPointTrigger)
        {
            _heroHolder = heroHolder;
            _finalPointTrigger = finalPointTrigger;
        }

        private readonly ReactiveVariable<bool> _isActive = new();
        public IReadOnlyVariable<bool> IsActive => _isActive;

        public void Initialize(Vector3 heroStartPosition)
        {
            _startPosition = heroStartPosition;
            _isInitialized = true;
            _progress.Value = 0f;
            _isActive.Value = true;
        }

        public void Reset()
        {
            _progress.Value = 0f;
            _isInitialized = false;
            _isActive.Value = false; 
        }

        public void Update(float deltaTime)
        {
            if (_isInitialized == false) 
                return;

            if (_heroHolder.MainHero == null) 
                return;

            Vector3 heroPos = _heroHolder.MainHero.Transform.position;
            Vector3 toFinish = _finalPointTrigger.FinalPointPosition - _startPosition;
            float totalDistance = toFinish.magnitude;

            if (totalDistance <= 0)
                return;

            Vector3 toHero = heroPos - _startPosition;

            float coveredDistance = Vector3.Dot(toHero, toFinish.normalized);

            _progress.Value = Mathf.Clamp01(coveredDistance / totalDistance);
        }
    }
}