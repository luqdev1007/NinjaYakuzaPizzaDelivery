using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeView : EntityView
    {
        private static readonly int IsPlungingKey = Animator.StringToHash("IsPlunging");

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("VFX - Flight (Ramp up)")]
        [SerializeField] private ParticleSystem _airConePS;
        [SerializeField] private ParticleSystem[] _fireCones;

        [Tooltip("Время до достижения максимальной силы эффектов")]
        [SerializeField] private float _fullPowerTime = 0.5f;
        [SerializeField] private float _maxAirEmission = 40f;
        [SerializeField] private float _maxFireEmission = 30f;

        [Header("VFX - Impact")]
        [SerializeField] private ParticleSystem _impactPS;

        [Header("Squash & Stretch")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _stretchY = 1.4f;
        [SerializeField] private float _squashY = 0.6f;
        [SerializeField] private float _squashDuration = 0.15f;
        [SerializeField] private float _lerpSpeed = 12f;

        private IReadOnlyVariable<bool> _isPlunging;
        private IReadOnlyVariable<bool> _isGrounded;

        private float _flightTimer;
        private bool _isSquashing;
        private Vector3 _defaultScale;

        private IDisposable _plungeDisposable;
        private IDisposable _groundedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;

            _plungeDisposable = _isPlunging.Subscribe(OnPlungeChanged);
            _groundedDisposable = _isGrounded.Subscribe(OnGroundedChanged);

            _animator.SetBool(IsPlungingKey, _isPlunging.Value);
        }

        private void Update()
        {
            if (!_isPlunging.Value) return;

            _flightTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(_flightTimer / _fullPowerTime);

            UpdateVFXPower(ratio);
            HandleStretch();
        }

        private void OnPlungeChanged(bool oldValue, bool isPlunging)
        {
            _animator.SetBool(IsPlungingKey, isPlunging);

            if (isPlunging)
            {
                _flightTimer = 0f;
                _airConePS?.Play();
            }
            else
            {
                StopFlightEffects();
            }
        }

        private void OnGroundedChanged(bool oldValue, bool grounded)
        {
            // Если коснулись земли и до этого падали хотя бы чуть-чуть
            if (grounded && _flightTimer > 0.05f)
            {
                float impactRatio = Mathf.Clamp(_flightTimer / _fullPowerTime, 0.2f, 1.5f);

                if (_impactPS != null)
                {
                    var main = _impactPS.main;
                    main.startSizeMultiplier = impactRatio;

                    var emission = _impactPS.emission;
                    var burst = emission.GetBurst(0);
                    burst.count = new ParticleSystem.MinMaxCurve(10 * impactRatio, 30 * impactRatio);
                    emission.SetBurst(0, burst);

                    _impactPS.Play();
                }

                StartSquash();
                StopFlightEffects();

                // Сбрасываем таймер, чтобы эффект не сработал повторно без нового Plunge
                _flightTimer = 0f;
            }
        }

        private void UpdateVFXPower(float ratio)
        {
            if (_airConePS != null)
            {
                var emission = _airConePS.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxAirEmission, ratio);
            }

            float fireRatio = Mathf.InverseLerp(0.4f, 1.0f, ratio);

            foreach (var ps in _fireCones)
            {
                if (ps == null) continue;

                var emission = ps.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxFireEmission, fireRatio);

                if (fireRatio > 0.05f && !ps.isPlaying) ps.Play();
                else if (fireRatio <= 0.05f && ps.isPlaying) ps.Stop();
            }
        }

        private void HandleStretch()
        {
            if (_viewContainer == null || _isSquashing) return;

            Vector3 targetScale = new Vector3(
                _defaultScale.x * (2f - _stretchY),
                _defaultScale.y * _stretchY,
                _defaultScale.z);

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);
        }

        private void StartSquash()
        {
            if (_viewContainer == null) return;

            _isSquashing = true;
            StopAllCoroutines();
            StartCoroutine(SquashRoutine());
        }

        private IEnumerator SquashRoutine()
        {
            Vector3 squashScale = new Vector3(
                _defaultScale.x * (2f - _squashY),
                _defaultScale.y * _squashY,
                _defaultScale.z);

            _viewContainer.localScale = squashScale;

            float elapsed = 0;

            while (elapsed < _squashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _squashDuration;
                _viewContainer.localScale = Vector3.Lerp(squashScale, _defaultScale, t);
                yield return null;
            }

            _viewContainer.localScale = _defaultScale;
            _isSquashing = false;
        }

        private void StopFlightEffects()
        {
            _airConePS?.Stop();

            foreach (var ps in _fireCones)
                ps?.Stop();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            StopFlightEffects();
            _plungeDisposable?.Dispose();
            _groundedDisposable?.Dispose();
        }
    }
}