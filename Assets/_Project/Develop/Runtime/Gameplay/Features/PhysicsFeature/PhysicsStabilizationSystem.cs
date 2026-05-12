using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class PhysicsStabilizationSystem : IEntitySystem, IInitializableSystem, IUpdatableSystem
    {
        private readonly float _stabilizationThreshold = 0.03f;
        private readonly float _rotationThreshold = 0.3f;

        private float _linearDrag;
        private float _angularDrag;
        private Rigidbody2D _linkedRigidbody;
        private ICompositeCondition _canAffect;

        public void OnInit(Entity entity)
        {
            /*
            _linearDrag = entity.LinearDrag.Value;
            _angularDrag = entity.AngularDrag.Value;
            _canAffect = entity.CanPhysicalyInteract;
            _linkedRigidbody = entity.Rigidbody;
            */
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canAffect.Evaluate() == false)
                return;

            if (_linkedRigidbody.linearVelocity.sqrMagnitude > 0)
            {
                _linkedRigidbody.linearVelocity = Vector2.MoveTowards(
                    _linkedRigidbody.linearVelocity,
                    Vector2.zero,
                    _linearDrag * deltaTime);

                if (_linkedRigidbody.linearVelocity.sqrMagnitude < _stabilizationThreshold)
                {
                    _linkedRigidbody.linearVelocity = Vector2.zero;
                }
            }

            if (Mathf.Abs(_linkedRigidbody.rotation) > _rotationThreshold || Mathf.Abs(_linkedRigidbody.angularVelocity) > 0)
            {
                _linkedRigidbody.angularVelocity = Mathf.MoveTowards(
                    _linkedRigidbody.angularVelocity,
                    0,
                    _angularDrag * deltaTime);

                float nextRotation = Mathf.MoveTowardsAngle(
                    _linkedRigidbody.rotation,
                    0,
                    _angularDrag * deltaTime);

                _linkedRigidbody.MoveRotation(nextRotation);

                if (Mathf.Abs(_linkedRigidbody.rotation) < _rotationThreshold)
                {
                    _linkedRigidbody.rotation = 0;
                    _linkedRigidbody.angularVelocity = 0;
                }
            }
        }
    }
}
