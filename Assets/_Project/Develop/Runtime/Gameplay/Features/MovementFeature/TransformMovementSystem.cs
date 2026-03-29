using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class TransformMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private ReactiveVariable<Vector2> _moveDirection;
        private ICompositeCondition _canMove;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<bool> _isMoving;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
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
                _transform.Translate(direction * _moveSpeed.Value * deltaTime, Space.World);
                _isMoving.Value = true;
            }
            else
            {
                _isMoving.Value = false;
            }
        }
    }
}