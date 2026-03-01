using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class TransformRotateWithLinearVelocitySystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody _rigidbody;

        private Transform _transform;

        private const float MinThresholdVelocityValue = 1f;

        public void OnInit(Entity entity)
        {
            // _rigidbody = entity.Rigidbody;

            _transform = entity.Transform;

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