using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Entity _entity;

        private const float AnomalousVelocityThreshold = 30f; // Поднял порог, чтобы Plunge не цеплял
        private const float MinUpwardVelocity = 0.5f;
        private const float RequiredTimeInAir = 1.2f; // Еще быстрее реакция на баг
        private const float DriveDuration = 2.5f;

        // Допуск изменения скорости (если скорость "замерла" на высоком значении)
        private const float VelocityStagnationThreshold = 0.1f;

        private float _accumulationTimer;
        private float _driveTimer;
        private bool _isDriveActive;
        private Vector2 _lastVelocity;

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

            Vector2 currentVelocity = _rigidbody.linearVelocity;
            bool isInAir = _entity.IsGrounded.Value == false;

            // 1. Игнорируем Plunge: Нам не интересен набор скорости ВНИЗ (Y < 0)
            bool isMovingUpOrSide = currentVelocity.y > MinUpwardVelocity || Mathf.Abs(currentVelocity.x) > AnomalousVelocityThreshold;

            // 2. Ловим аномалию: либо скорость выше порога (не вниз), 
            // либо скорость высокая и она почти не меняется (застрял в коллайдере)
            float velocityDelta = (currentVelocity - _lastVelocity).magnitude;
            bool isStagnantHighVelocity = currentVelocity.magnitude > 15f && velocityDelta < VelocityStagnationThreshold;
            bool isChaotic = currentVelocity.magnitude > AnomalousVelocityThreshold && isMovingUpOrSide;

            if (isInAir && (isChaotic || isStagnantHighVelocity))
            {
                // При жестком хаосе таймер летит вперед
                float multiplier = isChaotic ? 2.5f : 1.5f;
                _accumulationTimer += deltaTime * multiplier;

                if (_accumulationTimer >= RequiredTimeInAir)
                {
                    StartDrive();
                }
            }
            else
            {
                _accumulationTimer = Mathf.MoveTowards(_accumulationTimer, 0, deltaTime * 3f);
            }

            _lastVelocity = currentVelocity;
        }

        private void StartDrive()
        {
            _isDriveActive = true;
            _driveTimer = DriveDuration;

            if (_entity.IsAttackInvulnerable != null)
                _entity.IsAttackInvulnerable.Value = true;

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = true;

            // Мгновенно гасим вектор, который вызвал баг, до разумного предела
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, 10f);

            Debug.Log("<color=red>🛑 ОБНАРУЖЕНА ФИЗИЧЕСКАЯ АНОМАЛИЯ: АКТИВАЦИЯ ДРАЙВА</color>");
        }

        private void HandleDrive(float deltaTime)
        {
            _driveTimer -= deltaTime;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearDamping = 5f; // Увеличил демпфирование, чтобы быстрее "осадить" героя

            if (_driveTimer <= 0)
            {
                EndDrive();
            }
        }

        private void EndDrive()
        {
            _isDriveActive = false;
            _accumulationTimer = 0;

            _rigidbody.linearDamping = 0f;
            _rigidbody.gravityScale = 3f;

            _rigidbody.linearVelocity = Vector2.zero; // Полный стоп для безопасности

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = false;

            if (_entity.IsAttackInvulnerable != null)
                _entity.IsAttackInvulnerable.Value = false;

            Debug.Log("<color=green>✅ ФИЗИКА СТАБИЛИЗИРОВАНА</color>");
        }
    }
}