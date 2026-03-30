using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class DefeatState : EndGameState, IUpdatableState
    {
        private readonly GameplayPopupService _gameplayPopupService;
        private readonly AudioService _audioService;

        public DefeatState(
            IInputService inputService,
            GameplayPopupService gameplayPopupService,
            AudioService audioService) : base(inputService)
        {
            _gameplayPopupService = gameplayPopupService;
            _audioService = audioService;
        }

        public override void Enter()
        {
            base.Enter();

            // Не забудь добавить AudioService в зависимости конструктора
            _audioService.SetMusicMuted(true);

            _gameplayPopupService.OpenDefeatPopup();
        }

        public void Update(float deltaTime)
        {
        }
    }
}
