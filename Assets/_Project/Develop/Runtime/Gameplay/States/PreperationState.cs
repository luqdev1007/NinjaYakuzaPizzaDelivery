using Assets._Project.Develop.Runtime.Gameplay.Features.Mines;
using Assets._Project.Develop.Runtime.Gameplay.Features.Timers;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class PreperationState : State, IUpdatableState
    {
        private readonly GameplayTimersService _gameplayTimersService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;
        private readonly MinePlacementService _minePlacementService; 

        private float _time;

        public PreperationState
            (GameplayTimersService gameplayTimersService,
            GameplayScreenPresenter gameplayScreenPresenter,
            float time,
            MinePlacementService minePlacementService)
        {
            _gameplayTimersService = gameplayTimersService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
            _minePlacementService = minePlacementService;

            _time = 100;

            // _time = time;
        }

        public override void Enter()
        {
            base.Enter();

            _gameplayTimersService.CreatePreperationTimer(_time);
            _gameplayTimersService.PreperationTimer.Restart();

            _gameplayScreenPresenter.ShowPreperationTimer(_gameplayTimersService.PreperationTimer);
            _gameplayScreenPresenter.ShowUpgradesPanel();
        }

        public void Update(float deltaTime)
        {
            _minePlacementService.Update(deltaTime);
        }

        public override void Exit()
        {
            _minePlacementService.CancelPlacement();
            _gameplayScreenPresenter.HideUpgradesPanel();
            base.Exit();
        }
    }
}
