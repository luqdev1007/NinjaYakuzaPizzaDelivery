using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeRotationView : EntityView
    {
        [Header("Components")]
        [SerializeField] private Transform _viewContainer;

        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 5f;

        private Entity _entity;
        private bool _isInitialized;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_entity.IsOnSlope.Value)
            {
                Vector2 normal = _entity.SlopeNormal.Value;

                float targetAngle = Mathf.Atan2(normal.x, normal.y) * -Mathf.Rad2Deg;

                float facingSign = Mathf.Sign(_entity.LookDirectionX.Value);
                targetAngle *= facingSign;

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