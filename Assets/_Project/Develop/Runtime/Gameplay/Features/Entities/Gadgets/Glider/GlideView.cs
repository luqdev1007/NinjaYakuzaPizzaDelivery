using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideView : EntityView
    {
        private static readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [Header("VFX")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _airParticlesPS;
        [SerializeField] private Transform _viewContainer;

        [Header("Sway Settings")]
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private IReadOnlyVariable<bool> _isGliding;
        private IDisposable _isGlidingDisposable;

        private bool _isCurrentlyGliding;
        private float _swayTimer;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGliding = entity.IsGliding;
            _isGlidingDisposable = _isGliding.Subscribe(OnIsGlidingChanged);

            ApplyState(_isGliding.Value);
        }

        private void Update()
        {
            HandleSway();
        }

        private void OnIsGlidingChanged(bool oldValue, bool newValue)
        {
            ApplyState(newValue);
        }

        private void ApplyState(bool isGliding)
        {
            _isCurrentlyGliding = isGliding;
            _swayTimer = 0f;

            _animator.SetBool(IsGlidingKey, isGliding);

            if (isGliding)
                _airParticlesPS.Play();
            else
                _airParticlesPS.Stop();
        }

        private void HandleSway()
        {
            float targetZ = 0f;

            if (_isCurrentlyGliding)
            {
                _swayTimer += Time.deltaTime * _swaySpeed;
                targetZ = Mathf.Sin(_swayTimer) * _swayAngle;
            }

            _viewContainer.localRotation = Quaternion.Lerp(
                _viewContainer.localRotation,
                Quaternion.Euler(0, 0, targetZ),
                Time.deltaTime * _swayLerpSpeed
            );
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGlidingDisposable?.Dispose();
        }
    }
}