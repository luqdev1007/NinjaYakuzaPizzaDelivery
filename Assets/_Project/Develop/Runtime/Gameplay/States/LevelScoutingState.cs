using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Hints;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelScoutingState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly IInputService _inputService;
        private readonly GameplayPopupService _popupService;

        // Настройки (в идеале Config SO)
        private readonly float _moveSpeed = 15f;
        private readonly float _boostMultiplier = 2.5f;
        private readonly float _zoomSpeed = 5f;
        private readonly float _minZoom = 5f;
        private readonly float _maxZoom = 15f;

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


            _activeHint = _popupService.OpenHint("WASD - Move, Wheel - Zoom, Shift - Boost\nPress 'T' to start");
            _activeHint.Initialize();
        }

        public void Update(float deltaTime)
        {
            HandleMovement(deltaTime);
            HandleZoom(deltaTime);

            if (_inputService.IsStartLevelKeyPressed)
            {
                _isConfirmed = true;
            }
        }

        private void HandleMovement(float deltaTime)
        {
            Vector2 inputDirection = _inputService.CameraMoveDirection;

            if (inputDirection == Vector2.zero) 
                return;

            float currentSpeed = _moveSpeed;

            if (_inputService.IsDashKeyHeld)
            {
                currentSpeed *= _boostMultiplier;
            }

            Vector3 moveDelta = new Vector3(inputDirection.x, inputDirection.y, 0) * currentSpeed * deltaTime;
            _cameraService.ScoutingCamera.transform.position += moveDelta;
        }

        private void HandleZoom(float deltaTime)
        {
            float scroll = _inputService.MouseScrollDelta;

            if (Mathf.Abs(scroll) < 0.01f)
                return;

            var lens = _cameraService.ScoutingCamera.Lens;

            lens.OrthographicSize -= scroll * _zoomSpeed;
            lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize, _minZoom, _maxZoom);

            _cameraService.ScoutingCamera.Lens = lens;
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