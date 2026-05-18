using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move; 
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideView : EntityView
    {
        private static readonly int IsSlidingKey = Animator.StringToHash("IsSliding");

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("VFX - Friction & Dust")]
        [SerializeField] private ParticleSystem _frictionPS;
        [SerializeField] private float _maxEmissionRate = 50f;
        [SerializeField] private float _vfxRampUpTime = 0.3f;

        [Header("Visual Deformation")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _slideStretchX = 1.3f;
        [SerializeField] private float _slideSquashY = 0.7f;
        [SerializeField] private float _slideTiltAngle = 12f;
        [SerializeField] private float _lerpSpeed = 10f;
        [SerializeField] private float _slopeRotationLerp = 10f;

        private Entity _entity;
        private IReadOnlyVariable<bool> _isSliding;
        private IReadOnlyVariable<bool> _isOnSlope;
        private IReadOnlyVariable<MovementStates> _movementState;

        private float _slideTimer;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;

        private IDisposable _slideDisposable;
        private IDisposable _stateDisposable; 
        private bool _isInitialized;
        private bool _wasSlidingLastFrame;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;
            _isSliding = entity.IsSliding;
            _isOnSlope = entity.IsOnSlope;
            _movementState = entity.CurrentMovementState;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;
            _defaultRotation = _viewContainer != null ? _viewContainer.localRotation : Quaternion.identity;

            _slideDisposable = _isSliding.Subscribe((old, cur) => EvaluateSlidingState());
            _stateDisposable = _movementState.Subscribe((old, cur) => EvaluateSlidingState());

            _wasSlidingLastFrame = IsCurrentlySliding();
            _animator.SetBool(IsSlidingKey, _wasSlidingLastFrame);

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            HandleRotationAndDeformation();

            if (!IsCurrentlySliding())
                return;

            _slideTimer += Time.deltaTime;
            float intensity = Mathf.Clamp01(_slideTimer / _vfxRampUpTime);

            UpdateVFX(intensity);
        }

        private bool IsCurrentlySliding()
        {
            return _isSliding.Value || _movementState.Value == MovementStates.Sliding;
        }

        private void EvaluateSlidingState()
        {
            bool isSlidingNow = IsCurrentlySliding();

            if (isSlidingNow == _wasSlidingLastFrame)
                return;

            _wasSlidingLastFrame = isSlidingNow;
            _animator.SetBool(IsSlidingKey, isSlidingNow);

            if (isSlidingNow)
            {
                _slideTimer = 0f;
                if (_frictionPS != null) _frictionPS.Play();
            }
            else
            {
                StopEffects();
            }
        }

        private void HandleRotationAndDeformation()
        {
            if (_viewContainer == null)
                return;

            bool isSliding = IsCurrentlySliding();

            Vector3 targetScale = isSliding
                ? new Vector3(_defaultScale.x * _slideStretchX, _defaultScale.y * _slideSquashY, _defaultScale.z)
                : _defaultScale;

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);

            if (isSliding)
            {
                float targetZ = 0f;
                float direction = Mathf.Sign(_entity.LookDirectionX.Value);

                if (_isOnSlope.Value)
                {
                    targetZ = Vector2.SignedAngle(Vector2.up, _entity.SlopeNormal.Value);
                    targetZ += (direction > 0 ? -90f : 90f);
                }
                else
                {
                    targetZ = -_slideTiltAngle * direction;
                }

                Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRot, Time.deltaTime * _slopeRotationLerp);
            }
        }

        private void UpdateVFX(float intensity)
        {
            if (_frictionPS == null) return;

            var emission = _frictionPS.emission;
            emission.rateOverTime = Mathf.Lerp(5f, _maxEmissionRate, intensity);
        }

        private void StopEffects()
        {
            if (_frictionPS != null) _frictionPS.Stop();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isInitialized = false;

            StopEffects();
            _slideDisposable?.Dispose();
            _stateDisposable?.Dispose();

            if (_viewContainer != null)
            {
                _viewContainer.localScale = _defaultScale;
                _viewContainer.localRotation = _defaultRotation;
            }
        }
    }
}