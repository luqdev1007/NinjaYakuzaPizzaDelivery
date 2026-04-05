using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature
{
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        // Кэшированные ссылки на реактивные переменные
        private ReactiveVariable<bool> _isDriveActive;
        private ReactiveVariable<int> _driveJumps;
        private IReadOnlyVariable<bool> _isDashing;
        private IReadOnlyVariable<bool> _isThrowing;
        private IReadOnlyVariable<bool> _isGrounded;

        private Rigidbody2D _rigidbody;
        private float _driveDuration;
        private float _timer;
        private float _defaultGravity;

        public DriveSystem(IInputService inputService) => _inputService = inputService;

        public void OnInit(Entity entity)
        {
            // Кэшируем компоненты/переменные один раз
            _rigidbody = entity.Rigidbody;
            _isDriveActive = entity.IsDriveActive;
            _driveJumps = entity.DriveAvailableJumps;

            // Кэшируем переменные состояний других систем для быстрой проверки
            _isDashing = entity.IsDashing;
            _isThrowing = entity.IsThrowing;
            _isGrounded = entity.IsGrounded;

            _driveDuration = entity.DriveDuration.Value;
            _defaultGravity = 1f; // Можно также тянуть из entity.DefaultGravity, если есть
        }

        public void OnUpdate(float deltaTime)
        {
            // Используем только локальные закэшированные ссылки
            if (_isDriveActive.Value)
            {
                UpdateDriveState(deltaTime);
                return;
            }

            // Проверка "зависания" гравитации через кэшированные состояния
            if (_rigidbody.gravityScale < 0.1f && !_isDashing.Value && !_isThrowing.Value)
            {
                if (_rigidbody.linearVelocity.magnitude > 8f)
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
            _timer = _driveDuration;
            _rigidbody.gravityScale = 0f;
            _driveJumps.Value = 1;

            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private void UpdateDriveState(float deltaTime)
        {
            _timer -= deltaTime;

            if (_inputService.IsJumpKeyPressed && _driveJumps.Value > 0)
            {
                _driveJumps.Value--;
                // Используем Rigidbody напрямую (он уже в кэше)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 12f);
                ExitDrive();
                return;
            }

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
        }
    }
}