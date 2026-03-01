using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class TransformRotateWithLinearVelocitySystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private Rigidbody _rigidbody;

        private const float MinThresholdVelocityValue = 1f;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;

            _rigidbody.useGravity = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_rigidbody.linearVelocity.sqrMagnitude <= MinThresholdVelocityValue)
                return;

            _transform.rotation = Quaternion.LookRotation(_rigidbody.linearVelocity);
        }
    }
}