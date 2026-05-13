using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Hints;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelScoutingState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly IInputService _inputService;
        private readonly GameplayPopupService _popupService;

        private HintPresenter _activeHint;
        private bool _isConfirmed;

        public bool IsConfirmed => _isConfirmed;

        public LevelScoutingState(
            CameraService cameraService,
            IInputService inputService,
            GameplayPopupService popupService)
        {
            _cameraService = cameraService;
            _inputService = inputService;
            _popupService = popupService;
        }

        public override void Enter()
        {
            base.Enter();

            _isConfirmed = false;

            _cameraService.SetState(CameraState.Scouting);

            _activeHint = _popupService.OpenHint("Осмотрите уровень. Нажмите [Enter], чтобы начать.");

            _activeHint.Initialize();
        }

        public void Update(float deltaTime)
        {
            if (_inputService.IsStartLevelKeyPressed)
            {
                _isConfirmed = true;
            }
        }

        public override void Exit()
        {
            if (_activeHint != null)
            {
                _activeHint.Hide();
                _activeHint = null;
            }

            base.Exit();
        }
    }
}