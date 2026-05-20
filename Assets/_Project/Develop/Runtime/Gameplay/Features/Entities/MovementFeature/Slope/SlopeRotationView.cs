using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeRotationView : EntityView
    {
        [Header("Components")]
        [SerializeField] private Transform _viewContainer;

        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 15f; 

        private Entity _entity;
        private Transform _transform;
        private bool _isInitialized;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;
            _transform = entity.Transform;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized || _viewContainer == null)
                return;

            if (_entity.IsSliding.Value || _entity.CurrentMovementState.Value == MovementStates.Sliding)
                return;

            if (_entity.IsOnSlope.Value)
            {
                Vector3 worldNormal = _entity.SlopeNormal.Value;

                Vector3 localNormal = _transform.InverseTransformDirection(worldNormal);

                float targetAngle = Mathf.Atan2(localNormal.x, localNormal.y) * -Mathf.Rad2Deg;

                targetAngle = Mathf.Clamp(targetAngle, -60f, 60f);

                Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
            else
            {
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, Quaternion.identity, _rotationSpeed * Time.deltaTime);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isInitialized = false;

            if (_viewContainer != null)
            {
                _viewContainer.localRotation = Quaternion.identity;
            }
        }
    }
}
