using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move
{
    public class SimpleRigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private ReactiveVariable<Vector2> _moveDirection;
        private ICompositeCondition _canMove;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<bool> _isMoving;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _isMoving = entity.IsMoving;
            _canMove = entity.CanMove;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false)
                return;

            Vector2 direction = _moveDirection.Value;

            if (direction.sqrMagnitude > 0.001f)
            {
                _rigidbody.linearVelocity = direction * _moveSpeed.Value;
                _isMoving.Value = true;
            }
            else
            {
                _isMoving.Value = false;
            }
        }
    }
}