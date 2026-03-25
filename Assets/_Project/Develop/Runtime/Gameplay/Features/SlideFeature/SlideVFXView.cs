using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideVFXView : EntityView
    {
        [Header("VFX Components")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private ParticleSystem _frictionPS;

        [Header("Squash & Stretch Settings")]
        [SerializeField] private float _slideStretchX = 1.3f;
        [SerializeField] private float _slideSquashY = 0.7f;
        [SerializeField] private float _lerpSpeed = 12f;

        [Header("Tilt Settings")]
        [SerializeField] private float _slideTiltAngle = 10f; 

        private IReadOnlyVariable<bool> _isSliding;
        private IDisposable _isSlidingDisposable;

        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;
        private bool _isCurrentlySliding;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isSliding = entity.IsSliding;
            _isSlidingDisposable = _isSliding.Subscribe(OnSlideChanged);

            if (_viewContainer != null)
            {
                _defaultScale = _viewContainer.localScale;
                _defaultRotation = _viewContainer.localRotation;
            }
        }

        private void Update()
        {
            if (_viewContainer == null) return;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Vector3 targetScale = _defaultScale;
            float targetZRotation = 0f;

            if (_isCurrentlySliding)
            {
                targetScale = new Vector3(_defaultScale.x * _slideStretchX, _defaultScale.y * _slideSquashY, _defaultScale.z);

                float direction = Mathf.Sign(transform.localScale.x);
                targetZRotation = -_slideTiltAngle * direction;
            }

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZRotation);
            _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRotation, Time.deltaTime * _lerpSpeed);
        }

        private void OnSlideChanged(bool oldValue, bool value)
        {
            _isCurrentlySliding = value;

            if (value)
            {
                if (_frictionPS != null) _frictionPS.Play();
            }
            else
            {
                if (_frictionPS != null) _frictionPS.Stop();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isSlidingDisposable?.Dispose();

            if (_viewContainer != null)
            {
                _viewContainer.localScale = _defaultScale;
                _viewContainer.localRotation = _defaultRotation;
            }
        }
    }
}
