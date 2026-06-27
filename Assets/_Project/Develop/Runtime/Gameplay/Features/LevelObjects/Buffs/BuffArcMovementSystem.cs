using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffArcMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<bool> _isCollected;

        private float _elapsedTime;
        private Vector3 _startPosition;

        private readonly float _travelTime;
        private readonly float _arcHeight;

        public BuffArcMovementSystem(float travelTime, float arcHeight)
        {
            _travelTime = travelTime;
            _arcHeight = arcHeight;
        }

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _currentTarget = entity.CurrentTarget;
            _isCollected = entity.BuffIsCollected;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_currentTarget.Value == null || _isCollected.Value)
            {
                return;
            }

            if (_currentTarget.Value.Transform == null)
            {
                _currentTarget.Value = null;
                _elapsedTime = 0f;

                return;
            }

            if (_elapsedTime == 0f)
            {
                _startPosition = _transform.position;
            }

            _elapsedTime += deltaTime;

            float t = Mathf.Clamp01(_elapsedTime / _travelTime);
            float easeT = t * t * t;

            Vector3 targetPos = _currentTarget.Value.Transform.position;
            Vector3 lerpPosition = Vector3.Lerp(_startPosition, targetPos, easeT);

            float arc = Mathf.Sin(t * Mathf.PI) * _arcHeight;
            _transform.position = lerpPosition + new Vector3(0f, arc, 0f);

            if (t >= 1f)
            {
                _transform.position = targetPos;
            }
        }
    }
}