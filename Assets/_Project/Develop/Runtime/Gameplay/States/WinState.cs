using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class WinState : EndGameState, IUpdatableState
    {
        private readonly GameplayPopupService _gameplayPopupService;

        public WinState(
            IInputService inputService,
            GameplayScreenPresenter gameplayScreenPresenter,
            GameplayPopupService gameplayPopupService) : base(inputService, gameplayScreenPresenter)
        {
            _gameplayPopupService = gameplayPopupService;
        }

        public override void Enter()
        {
            base.Enter();

            _gameplayPopupService.OpenWinMenuPopup();
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}
