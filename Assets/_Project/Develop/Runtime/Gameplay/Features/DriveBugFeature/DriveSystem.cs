using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature
{
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly CameraService _cameraService;

        private ReactiveVariable<bool> _isDriveActive;
        private ReactiveVariable<int> _driveJumps;
        private IReadOnlyVariable<bool> _isDashing;
        private IReadOnlyVariable<bool> _isThrowing;
        private IReadOnlyVariable<bool> _isGrounded;

        private Rigidbody2D _rigidbody;
        private float _defaultGravity;

        // --- ТАЙМЕР И НАСТРОЙКИ ---
        private float _timer;
        private const float MaxDriveDuration = 2.0f; // Максимальное время в полете (в реальных секундах)
        private const float DriveTimeScale = 0.1f;    // Насколько сильно замедляем
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
            _isThrowing = entity.IsThrowing;
            _isGrounded = entity.IsGrounded;

            _defaultGravity = entity.Rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDriveActive.Value)
            {
                // ВАЖНО: deltaTime здесь уже замедленный, поэтому таймер будет идти "медленно"
                // Если нужно ограничение в реальных секундах, используй Time.unscaledDeltaTime
                UpdateDriveState(Time.unscaledDeltaTime);
                return;
            }

            if (_rigidbody.gravityScale < 0.5f && !_isDashing.Value && !_isThrowing.Value)
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
            _timer = MaxDriveDuration; // Сбрасываем таймер при входе

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

            // Визуальный фидбек: можно слегка "потряхивать" зум чаще, когда время на исходе
            if (Time.frameCount % 5 == 0)
                _cameraService.ZoomImpulse(0.3f);

            // 1. Выход по прыжку
            if (_inputService.IsJumpKeyPressed && _driveJumps.Value > 0)
            {
                _driveJumps.Value--;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 14f);
                ExitDrive();
                return;
            }

            // 2. Авто-выход по таймеру или касанию земли
            if (_timer <= 0 || _isGrounded.Value)
            {
                ExitDrive();
            }
        }

        private void ExitDrive()
        {
            _isDriveActive.Value = false;
            _rigidbody.gravityScale = _defaultGravity;

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            _cameraService.Shake(0.15f);
        }
    }
}