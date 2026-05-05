using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class HeroSurfaceAlignmentView : EntityView
    {
        [Header("Transform Targets")]
        [SerializeField] private Transform _viewContainer;

        [Header("Slide Deformation")]
        [SerializeField] private float _stretchX = 1.3f;
        [SerializeField] private float _squashY = 0.7f;
        [SerializeField] private float _lerpSpeed = 10f;

        [Header("Rotation")]
        [SerializeField] private float _baseTiltAngle = 12f;
        [SerializeField] private float _rotationSmoothness = 0.15f;

        private Entity _linkedEntity;
        private SlopeSystem _slopeSystem;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _slopeSystem = entity.GetSystem<SlopeSystem>();
            _defaultScale = _viewContainer.localScale;
            _defaultRotation = _viewContainer.localRotation;
        }

        private void Update()
        {
            if (_viewContainer == null)
                return;

            bool isSliding = _linkedEntity.IsSliding.Value;
            bool isOnSlope = _linkedEntity.IsOnSlope.Value;

            HandleScale(isSliding);
            HandleRotation(isSliding, isOnSlope);
        }

        private void HandleScale(bool isSliding)
        {
            Vector3 targetScale = isSliding
                ? new Vector3(_defaultScale.x * _stretchX, _defaultScale.y * _squashY, _defaultScale.z)
                : _defaultScale;

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);
        }

        private void HandleRotation(bool isSliding, bool isOnSlope)
        {
            float targetZ = 0f;
            float direction = Mathf.Sign(transform.localScale.x);

            if (isSliding)
            {
                if (isOnSlope && _slopeSystem != null)
                {
                    targetZ = Vector2.SignedAngle(Vector2.up, _slopeSystem.SlopeNormal);
                    targetZ += (direction > 0 ? -90f : 90f);
                }
                else
                {
                    targetZ = -_baseTiltAngle * direction;
                }
            }

            Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
            _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRot, _rotationSmoothness);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _viewContainer.localScale = _defaultScale;
            _viewContainer.localRotation = _defaultRotation;
        }
    }
}