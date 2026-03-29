using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Entity _entity;

        private const float MinUpwardVelocity = 0.1f;
        private const float RequiredTimeInAir = 4.0f;
        private const float DriveDuration = 3.0f;

        private float _accumulationTimer;
        private float _driveTimer;
        private bool _isDriveActive;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDriveActive)
            {
                HandleDrive(deltaTime);
                return;
            }

            bool isInAir = _entity.IsGrounded.Value == false;
            bool isMovingUp = _rigidbody.linearVelocity.y > MinUpwardVelocity;

            if (isInAir && isMovingUp)
            {
                _accumulationTimer += deltaTime;

                if (_accumulationTimer >= RequiredTimeInAir)
                {
                    StartDrive();
                }
            }
            else
            {
                _accumulationTimer = Mathf.MoveTowards(_accumulationTimer, 0, deltaTime * 3f);
            }
        }

        private void StartDrive()
        {
            _isDriveActive = true;
            _driveTimer = DriveDuration;

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = true;

            Debug.Log("<color=orange>ОБНАРУЖЕН АНОМАЛЬНЫЙ ПОЛЕТ!</color>");
        }

        private void HandleDrive(float deltaTime)
        {
            _driveTimer -= deltaTime;

            Debug.Log("<color=red>ДРАЙВ!</color> " + _driveTimer.ToString("F1"));

            if (_driveTimer <= 0)
            {
                EndDrive();
            }
        }

        private void EndDrive()
        {
            _isDriveActive = false;
            _accumulationTimer = 0;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = false;

            Debug.Log("<color=cyan>СТАБИЛИЗАЦИЯ ПОТОКА ЗАВЕРШЕНА.</color>");
        }
    }
}