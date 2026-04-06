using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature
{
    // NOT WORK!
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly CameraService _cameraService;

        private ReactiveVariable<bool> _isDriveActive;
        private ReactiveVariable<int> _driveJumps;
        private IReadOnlyVariable<bool> _isDashing;
        private IReadOnlyVariable<bool> _isGliding;
        private IReadOnlyVariable<bool> _isThrowing;
        private IReadOnlyVariable<bool> _isGrounded;

        private Rigidbody2D _rigidbody;
        private float _defaultGravity;

        private float _timer;
        private const float MaxDriveDuration = 3.0f;
        private const float DriveTimeScale = 0.1f;
        private const float DriveZoomIntensity = 1.0f;

        public DriveSystem(IInputService inputService, CameraService cameraService)
        {
            _inputService = inputService;
            _cameraService = cameraService;
        }

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isDriveActive = entity.IsDriveActive;
            _driveJumps = entity.DriveAvailableJumps;
            _isDashing = entity.IsDashing;
            _isGliding = entity.IsGliding;
            _isThrowing = entity.IsThrowing;
            _isGrounded = entity.IsGrounded;
            _defaultGravity = entity.Rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDriveActive.Value)
            {
                UpdateDriveState(Time.unscaledDeltaTime);
                return;
            }

            if (_rigidbody.gravityScale < 0.5f && !_isDashing.Value && !_isThrowing.Value && !_isGliding.Value)
            {
                if (_rigidbody.linearVelocity.magnitude > 8f && !_isGrounded.Value)
                {
                    ActivateDrive();
                }
                else
                {
                    _rigidbody.gravityScale = _defaultGravity;
                }
            }
        }

        private void ActivateDrive()
        {
            _isDriveActive.Value = true;
            _timer = MaxDriveDuration;

            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _driveJumps.Value = 1;

            Time.timeScale = DriveTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            _cameraService.ZoomImpulse(DriveZoomIntensity);
            _cameraService.Shake(0.2f);
        }

        private void UpdateDriveState(float unscaledDeltaTime)
        {
            _timer -= unscaledDeltaTime;

            if (Time.frameCount % 5 == 0)
                _cameraService.ZoomImpulse(0.3f);

            if (_inputService.IsJumpKeyPressed && _driveJumps.Value > 0)
            {
                _driveJumps.Value--;
                ExecuteDriveJump(); // Придаем импульс прямо здесь
                return;
            }

            if (_timer <= 0 || _isGrounded.Value)
            {
                ExitDrive();
            }
        }

        private void ExecuteDriveJump()
        {
            // Рассчитываем направление вылета
            float horizontalInput = _inputService.MoveDirection.x;

            // Базовый мощный импульс: вверх + в сторону
            Vector2 launchForce = new Vector2(horizontalInput * 18f, 16f);

            // Сбрасываем скорость перед вылетом (на всякий случай)
            _rigidbody.linearVelocity = Vector2.zero;

            // Сначала выходим из драйва, чтобы вернулось время и гравитация
            ExitDrive();

            // ПРИКЛАДЫВАЕМ СИЛУ (теперь в нормальном времени)
            _rigidbody.AddForce(launchForce, ForceMode2D.Impulse);

            _cameraService.Shake(0.5f);
            _cameraService.ZoomImpulse(1.4f);
        }

        private void ExitDrive()
        {
            _isDriveActive.Value = false;
            _rigidbody.gravityScale = _defaultGravity;

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}