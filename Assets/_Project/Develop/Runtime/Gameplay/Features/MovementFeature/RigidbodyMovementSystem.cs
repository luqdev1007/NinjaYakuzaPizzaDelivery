using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody _rigidbody;

        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ICondition _canMove;

        public void OnInit(Entity entity)
        {
            // _rigidbody = entity.Rigidbody;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _canMove = entity.CanMove;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false)
            {
                return;
            }

            Vector3 velocity = _moveDirection.Value * _moveSpeed.Value;
            _rigidbody.linearVelocity = velocity;
        }
    }
}
