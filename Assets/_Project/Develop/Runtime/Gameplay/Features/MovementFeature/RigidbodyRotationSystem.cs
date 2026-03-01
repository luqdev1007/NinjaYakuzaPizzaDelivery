using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyRotationSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody _rigidbody;

        private ReactiveVariable<Vector3> _direction;
        private ReactiveVariable<float> _rotationSpeed;
        private ICompositeCondition _canRotate;

        public void OnInit(Entity entity)
        {
            // _rigidbody = entity.Rigidbody;

            _direction = entity.RotationDirection;
            _rotationSpeed = entity.RotationSpeed;
            _canRotate = entity.CanRotate;

            Vector3 startDir = _direction.Value;
            startDir.y = 0;

            if (startDir.sqrMagnitude > 0.001f)
            {
                _rigidbody.rotation = Quaternion.LookRotation(startDir.normalized);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canRotate.Evaluate() == false)
            {
                return;
            }

            Vector3 direction = _direction.Value;

            direction.y = 0;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

            if (Quaternion.Angle(_rigidbody.rotation, lookRotation) < 0.1f)
                return;

            float step = _rotationSpeed.Value * deltaTime;

            Quaternion rotation = Quaternion.RotateTowards(_rigidbody.rotation, lookRotation, step);

            _rigidbody.MoveRotation(rotation);
        }
    }
}