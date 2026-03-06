using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{

    public class RigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private Rigidbody2D _rigidbody;
        private ReactiveVariable<bool> _isMoving;

        private ICompositeCondition _canMove;

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _rigidbody = entity.Rigidbody;
            _canMove = entity.CanMove;
            _isMoving = entity.IsMoving;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false)
            {
                _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
                return;
            }

            // сохраняем Y velocity (гравитация + прыжок), меняем только X
            float targetX = _moveDirection.Value.normalized.x * _moveSpeed.Value;
            _rigidbody.linearVelocity = new Vector2(targetX, _rigidbody.linearVelocity.y);

            _isMoving.Value = Mathf.Abs(_rigidbody.linearVelocity.x) > 0.01f;
        }
    }
}
