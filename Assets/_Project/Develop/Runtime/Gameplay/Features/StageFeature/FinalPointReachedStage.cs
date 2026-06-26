using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class FinalPointReachedStage : IStage
    {
        private readonly FinalPointTriggerService _finalPointTrigger;
        private readonly MainHeroHolderService _heroHolder;

        private readonly ReactiveEvent _completed = new();

        private bool _inProcess;

        public IReadOnlyEvent Completed => _completed;

        public FinalPointReachedStage(
            FinalPointTriggerService finalPointTrigger,
            MainHeroHolderService heroHolder)
        {
            _finalPointTrigger = finalPointTrigger;
            _heroHolder = heroHolder;
        }

        public void Start()
        {
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
        }

        public void Dispose() => Cleanup();

        private void ProcessEnd()
        {
            _inProcess = false;
            _completed.Invoke();
        }
    }
}