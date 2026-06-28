using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public abstract class EndGameState : State
    {
        private readonly IInputService _inputService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;

        protected EndGameState(IInputService inputService, GameplayScreenPresenter gameplayScreenPresenter)
        {
            _inputService = inputService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
        }

        public override void Enter()
        {
            base.Enter();

            _inputService.IsEnabled = false;
            _gameplayScreenPresenter.StopGameplayHud();
        }

        public override void Exit()
        {
            _inputService.IsEnabled = true;

            base.Exit();
        }
    }
}