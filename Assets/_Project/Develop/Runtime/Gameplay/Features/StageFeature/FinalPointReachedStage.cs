using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class FinalPointReachedStage : IStage
    {
        private readonly FinalPointTriggerService _finalPointTrigger;
        private readonly MainHeroHolderService _heroHolder;
        private readonly Vector3 _finalPointPosition;
        private readonly ReactiveEvent _completed = new();

        private bool _inProcess;
        private bool _isProgressInitialized;

        public IReadOnlyEvent Completed => _completed;

        public FinalPointReachedStage(
            FinalPointTriggerService finalPointTrigger,
            MainHeroHolderService heroHolder,
            Vector3 finalPointPosition)
        {
            _finalPointTrigger = finalPointTrigger;
            _heroHolder = heroHolder;
            _finalPointPosition = finalPointPosition;
        }

        public void Start()
        {
            _finalPointTrigger.Create(_finalPointPosition);

            _inProcess = true;
        }

        public void Update(float deltaTime)
        {
            if (_inProcess == false)
                return;

            _finalPointTrigger.Update(deltaTime);

            if (_finalPointTrigger.HasMainHeroContact.Value)
                ProcessEnd();
        }

    
        public void Cleanup()
        {
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