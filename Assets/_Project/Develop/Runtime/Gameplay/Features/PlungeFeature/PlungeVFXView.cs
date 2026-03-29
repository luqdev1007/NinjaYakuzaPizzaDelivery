using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeVFXView : EntityView
    {
        [Header("Air Cone")]
        [SerializeField] private ParticleSystem _airConePS;

        [Header("Fire Cones (High Speed)")]
        [SerializeField] private ParticleSystem[] _fireCones; // Теперь массив
        [SerializeField] private float _fireSpeedThresholdMultiplier = 1.2f;

        [Header("Impact")]
        [SerializeField] private ParticleSystem _impactPS;

        [Header("Squash Stretch")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _stretchY = 1.4f;
        [SerializeField] private float _squashY = 0.6f;
        [SerializeField] private float _squashDuration = 0.15f;
        [SerializeField] private float _stretchLerpSpeed = 10f;
        [SerializeField] private float _recoveryLerpSpeed = 8f;

        private IReadOnlyVariable<bool> _isPlunging;
        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<float> _basePlungeSpeed;

        private Rigidbody2D _rigidbody;
        private IDisposable _isPlungingDisposable;
        private IDisposable _isGroundedDisposable;

        private bool _plunging;
        private bool _wasPlunging;
        private bool _squashing;
        private float _squashTimer;
        private Vector3 _defaultScale;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;
            _basePlungeSpeed = entity.PlungeSpeed;
            _rigidbody = entity.Rigidbody;

            _isPlungingDisposable = _isPlunging.Subscribe(OnIsPlungingChanged);
            _isGroundedDisposable = _isGrounded.Subscribe(OnIsGroundedChanged);

            if (_viewContainer != null)
                _defaultScale = _viewContainer.localScale;
        }

        private void Update()
        {
            if (_viewContainer == null) return;

            HandleFireCones();
            HandleSquashStretch();
        }

        private void HandleFireCones()
        {
            if (_fireCones == null || _fireCones.Length == 0) return;

            // Проверка превышения скорости
            bool isSuperFast = _plunging &&
                               Mathf.Abs(_rigidbody.linearVelocity.y) > (_basePlungeSpeed.Value * _fireSpeedThresholdMultiplier);

            foreach (var ps in _fireCones)
            {
                if (ps == null) continue;

                if (isSuperFast && !ps.isPlaying)
                    ps.Play();
                else if (!isSuperFast && ps.isPlaying)
                    ps.Stop();
            }
        }

        private void HandleSquashStretch()
        {
            if (_squashing)
            {
                _squashTimer += Time.deltaTime;
                float t = _squashTimer / _squashDuration;

                Vector3 targetScale = Vector3.Lerp(
                    new Vector3(_defaultScale.x * (2f - _squashY), _defaultScale.y * _squashY, _defaultScale.z),
                    _defaultScale,
                    t);

                _viewContainer.localScale = targetScale;

                if (_squashTimer >= _squashDuration)
                    _squashing = false;

                return;
            }

            if (_plunging)
            {
                Vector3 targetScale = new Vector3(
                    _defaultScale.x * (2f - _stretchY),
                    _defaultScale.y * _stretchY,
                    _defaultScale.z);

                _viewContainer.localScale = Vector3.Lerp(
                    _viewContainer.localScale,
                    targetScale,
                    Time.deltaTime * _stretchLerpSpeed);
            }
            else
            {
                _viewContainer.localScale = Vector3.Lerp(
                    _viewContainer.localScale,
                    _defaultScale,
                    Time.deltaTime * _recoveryLerpSpeed);
            }
        }

        private void OnIsPlungingChanged(bool oldValue, bool value)
        {
            _plunging = value;

            if (value)
            {
                _wasPlunging = true;
                if (_airConePS != null) _airConePS.Play();
            }
            else
            {
                if (_airConePS != null) _airConePS.Stop();
                StopAllFireCones();
            }
        }

        private void OnIsGroundedChanged(bool oldValue, bool value)
        {
            if (value && _wasPlunging)
            {
                if (_impactPS != null) _impactPS.Play();
                StopAllFireCones();

                _squashing = true;
                _squashTimer = 0f;
                _wasPlunging = false;
            }
        }

        private void StopAllFireCones()
        {
            if (_fireCones == null) return;
            foreach (var ps in _fireCones)
            {
                if (ps != null) ps.Stop();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isPlungingDisposable?.Dispose();
            _isGroundedDisposable?.Dispose();

            if (_viewContainer != null)
                _viewContainer.localScale = _defaultScale;
        }
    }
}