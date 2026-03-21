using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideVFXView : EntityView
    {
        [Header("Air Particles")]
        [SerializeField] private ParticleSystem _airParticlesPS;

        [Header("Sway")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private IReadOnlyVariable<bool> _isGliding;
        private IDisposable _isGlidingDisposable;
        private bool _gliding;
        private float _swayTimer;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGliding = entity.IsGliding;
            _isGlidingDisposable = _isGliding.Subscribe(OnIsGlidingChanged);
        }

        private void Update()
        {
            if (!_gliding)
            {
                if (_viewContainer != null)
                {
                    Vector3 euler = _viewContainer.localEulerAngles;
                    float currentZ = euler.z > 180f ? euler.z - 360f : euler.z;
                    float newZ = Mathf.Lerp(currentZ, 0f, Time.deltaTime * _swayLerpSpeed);
                    _viewContainer.localEulerAngles = new Vector3(0f, euler.y, newZ);
                }
                return;
            }

            _swayTimer += Time.deltaTime * _swaySpeed;
            float targetAngle = Mathf.Sin(_swayTimer) * _swayAngle;

            if (_viewContainer != null)
            {
                Vector3 euler = _viewContainer.localEulerAngles;
                float currentZ = euler.z > 180f ? euler.z - 360f : euler.z;
                float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * _swayLerpSpeed);
                _viewContainer.localEulerAngles = new Vector3(0f, euler.y, newZ);
            }
        }

        private void OnIsGlidingChanged(bool oldValue, bool value)
        {
            _gliding = value;
            _swayTimer = 0f;

            if (_airParticlesPS == null)
                return;

            if (value)
                _airParticlesPS.Play();
            else
                _airParticlesPS.Stop();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGlidingDisposable?.Dispose();
        }
    }
}