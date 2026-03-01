using Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class StageProcessState : State, IUpdatableState
    {
        private readonly StageProviderService _stageProviderService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;

        public StageProcessState(
            StageProviderService stageProviderService, 
            GameplayScreenPresenter gameplayScreenPresenter)
        {
            _stageProviderService = stageProviderService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
        }

        public override void Enter()
        {
            base.Enter();

            _gameplayScreenPresenter.ShowAnnouncement("СВИСТАТЬ ВСЕХ НАВЕРХ!", "Готовьтесь к бою!");
            _stageProviderService.SwitchToNext();
            _stageProviderService.StartCurrent();
        }

        public void Update(float deltaTime)
        {
            _stageProviderService.UpdateCurrent(deltaTime);
        }

        public override void Exit()
        {
            base.Exit();

            _stageProviderService.CleanupCurrent();
        }
    }
}
