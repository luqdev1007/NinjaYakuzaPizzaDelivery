using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyDirectionalRotatorSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _rotationSpeed;
        private Rigidbody _rigidbody;
        private ICompositeCondition _canRotate;

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _rigidbody = entity.Rigidbody;
            _canRotate = entity.CanRotate;
            _rotationSpeed = entity.RotationSpeed;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_rigidbody == null || _canRotate.Evaluate() == false)
                return;

            if (_moveDirection.Value.sqrMagnitude <= 0.01f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection.Value);

            Quaternion smoothRotation = Quaternion.Slerp(
                _rigidbody.rotation, 
                targetRotation, 
                deltaTime * _rotationSpeed.Value
            );
            _rigidbody.MoveRotation(smoothRotation);
        }
    }
}