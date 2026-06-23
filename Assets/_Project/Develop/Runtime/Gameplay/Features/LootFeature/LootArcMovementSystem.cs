using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootArcMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<bool> _isCollected;
        private ICompositeCondition _canMove;

        private float _elapsedTime;
        private Vector3 _startPosition;

        private readonly float _travelTime;
        private readonly float _arcHeight;

        public LootArcMovementSystem(float travelTime, float arcHeight)
        {
            _travelTime = travelTime;
            _arcHeight = arcHeight;
        }

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _currentTarget = entity.CurrentTarget;
            _isCollected = entity.LootIsCollected;
            _canMove = entity.CanMove;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove != null && _canMove.Evaluate() == false) 
                return;

            if (_currentTarget.Value == null || _isCollected.Value) 
                return;

            if (_elapsedTime == 0)
            {
                _startPosition = _transform.position;
            }

            _elapsedTime += deltaTime;

            float t = Mathf.Clamp01(_elapsedTime / _travelTime);
            float easeT = t * t * t;

            Vector3 targetPos = _currentTarget.Value.Transform.position;
            Vector3 lerpPosition = Vector3.Lerp(_startPosition, targetPos, easeT);

 
            float arc = Mathf.Sin(t * Mathf.PI) * _arcHeight;
            _transform.position = lerpPosition + new Vector3(0, arc, 0);

            if (t >= 1.0f)
            {
                _transform.position = targetPos;
            }
        }
    }
}