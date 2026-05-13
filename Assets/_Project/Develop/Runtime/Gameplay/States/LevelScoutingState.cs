using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelScoutingState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly IInputService _inputService;
        private bool _isConfirmed;

        public bool IsConfirmed => _isConfirmed;

        public LevelScoutingState(CameraService cameraService, IInputService inputService)
        {
            _cameraService = cameraService;
            _inputService = inputService;
        }

        public override void Enter()
        {
            base.Enter();

            _isConfirmed = false;
            _cameraService.SetState(CameraState.Scouting);
        }

        public void Update(float deltaTime)
        {
            if (_inputService.IsStartLevelKeyPressed == true)
            {
                _isConfirmed = true;
            }
        }
    }
}