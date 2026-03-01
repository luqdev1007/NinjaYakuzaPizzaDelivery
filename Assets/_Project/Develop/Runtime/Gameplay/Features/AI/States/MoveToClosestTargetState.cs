using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class MoveToClosestTargetState : State, IUpdatableState
    {
        private readonly ReactiveVariable<Vector3> _movementDirection;
        private readonly ReactiveVariable<Vector3> _rotationDirection;
        private readonly ReactiveVariable<Entity> _target;
        private readonly Transform _transform;

        public MoveToClosestTargetState(Entity entity)
        {
            _movementDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
            _target = entity.CurrentTarget;
            _transform = entity.Transform;
        }

        public override void Enter()
        {
            _movementDirection.Value = Vector3.zero;
        }

        public override void Exit()
        {
            _movementDirection.Value = Vector3.zero;
            _rotationDirection.Value = Vector3.zero;
        }

        public void Update(float deltaTime)
        {
            if (_target.Value == null || _target.Value.Transform == null)
            {
                _movementDirection.Value = Vector3.zero;
                return;
            }

            Vector3 targetPosition = _target.Value.Transform.position;
            Vector3 currentPosition = _transform.position;

            Vector3 direction = (targetPosition - currentPosition).normalized;
            direction.y = 0;

            _movementDirection.Value = direction;
            _rotationDirection.Value = direction;
        }
    }
}