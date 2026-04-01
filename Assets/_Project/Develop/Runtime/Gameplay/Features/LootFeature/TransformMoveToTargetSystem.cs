using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class TransformMoveToTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float TravelTime = 1.5f;

        private Transform _transform;
        private ReactiveVariable<Entity> _currentTarget;
        private ICompositeCondition _canMove;

        private float _elapsedTime;
        private Vector3 _startPosition;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _currentTarget = entity.CurrentTarget;
            _canMove = entity.CanMove;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false || _currentTarget.Value == null)
            {
                _elapsedTime = 0;
                return;
            }

            if (_elapsedTime == 0)
                _startPosition = _transform.position;

            _elapsedTime += deltaTime;

            float t = Mathf.Clamp01(_elapsedTime / TravelTime);
            float easeT = t * t * t;

            _transform.position = Vector3.Lerp(_startPosition, _currentTarget.Value.Transform.position, easeT);
        }
    }
}