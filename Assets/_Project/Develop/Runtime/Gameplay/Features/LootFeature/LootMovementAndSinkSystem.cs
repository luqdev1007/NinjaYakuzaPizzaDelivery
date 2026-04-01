using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMovementAndSinkSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<bool> _isPulling;
        private ReactiveVariable<bool> _isCollected;
        private ReactiveVariable<bool> _inSpawnProcess;

        private float _lifeTimer = 0f;
        private float _spawnTimer = 0f;
        private float _pullAcceleration = 0f;

        private readonly float _maxLifeTime = 3f;
        private readonly float _startPullSpeed = 2f;
        private readonly float _spawnDelay = 0.4f; // Задержка перед началом магнита

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _moveDirection = entity.MoveDirection;
            _isPulling = entity.IsPullingProcess;
            _isCollected = entity.IsCollected;
            _inSpawnProcess = entity.InSpawnProcess;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isCollected.Value) return;

            // 1. Таймер разлета (блокирует магнит)
            if (_inSpawnProcess.Value)
            {
                _spawnTimer += deltaTime;
                if (_spawnTimer >= _spawnDelay)
                {
                    _inSpawnProcess.Value = false;
                }
                return; // Пока вылетаем, остальная логика (магнит/удаление) ждет
            }

            _lifeTimer += deltaTime;

            // 2. Логика магнита (ускорение)
            if (_isPulling.Value)
            {
                _pullAcceleration += deltaTime * 18f;
                float currentSpeed = _startPullSpeed + _pullAcceleration;
                _transform.position += (Vector3)_moveDirection.Value * currentSpeed * deltaTime;
            }

            // 3. Автоудаление
            if (_lifeTimer >= _maxLifeTime && !_isPulling.Value)
            {
                _isCollected.Value = true;
            }
        }
    }
}