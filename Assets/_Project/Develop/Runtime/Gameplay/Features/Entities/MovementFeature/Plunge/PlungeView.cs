using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge
{
    public class PlungeView : EntityView, IRequireAudioService
    {
        private static readonly int IsPlungingKey = Animator.StringToHash("IsPlunging");

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("VFX - Flight (Ramp up)")]
        [SerializeField] private ParticleSystem _airConePS;
        [SerializeField] private ParticleSystem[] _fireCones;

        [Tooltip("Устарело: теперь все зависит от физической скорости персонажа")]
        [SerializeField] private float _maxAirEmission = 40f;
        [SerializeField] private float _maxFireEmission = 30f;

        [Header("VFX - Impact")]
        [SerializeField] private ParticleSystem _impactPS;

        [Header("SFX Keys")]
        [SerializeField] private string _loopSfxKey = "AbilityImpactPlungeLoop";
        [SerializeField] private string _landSfxKey = "AbilityImpactPlunge";

        [Header("Squash & Stretch")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _stretchY = 1.4f;
        [SerializeField] private float _squashY = 0.6f;
        [SerializeField] private float _squashDuration = 0.15f;
        [SerializeField] private float _lerpSpeed = 12f;

        private IAudioService _audioService;
        private Entity _linkedEntity;
        private IReadOnlyVariable<bool> _isPlunging;

        private bool _isSquashing;
        private Vector3 _defaultScale;
        private string _activeLoopKey;

        private IDisposable _plungeDisposable;
        private IDisposable _impactDisposable;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _isPlunging = entity.IsPlunging;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;

            _plungeDisposable = _isPlunging.Subscribe(OnPlungeChanged);
            _impactDisposable = _linkedEntity.PlungeImpactEvent.Subscribe(OnPlungeImpact);

            _animator.SetBool(IsPlungingKey, _isPlunging.Value);
        }

        private void Update()
        {
            if (!_isPlunging.Value)
                return;

            float currentSpeed = Mathf.Abs(_linkedEntity.Rigidbody.linearVelocity.y);
            float minImpactSpeed = _linkedEntity.MinPlungeImpactSpeedThreshold.Value;
            float maxSpeed = _linkedEntity.PlungeSpeed.Value;

            float airRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

            float fireRatio = 0f;
            if (currentSpeed > minImpactSpeed)
            {
                fireRatio = Mathf.Clamp01((currentSpeed - minImpactSpeed) / (maxSpeed - minImpactSpeed));
            }

            UpdateVFXPower(airRatio, fireRatio);
            HandleStretch();
        }

        private void OnPlungeChanged(bool oldValue, bool isPlunging)
        {
            _animator.SetBool(IsPlungingKey, isPlunging);

            if (isPlunging)
            {
                _airConePS?.Play();
                _activeLoopKey = _loopSfxKey;
                _audioService.PlaySfxLoop(_activeLoopKey, transform.position);
            }
            else
            {
                StopFlightEffects();
            }
        }

        private void OnPlungeImpact(float finalSpeed)
        {
            float baseSpeed = _linkedEntity.PlungeSpeed.Value;
            float impactRatio = Mathf.Clamp(finalSpeed / (baseSpeed > 0 ? baseSpeed : 12f), 0.5f, 2.0f);

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

            _audioService.PlaySfx(_landSfxKey, transform.position);
            StartSquash();
        }

        private void UpdateVFXPower(float airRatio, float fireRatio)
        {
            if (_airConePS != null)
            {
                var emission = _airConePS.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxAirEmission, airRatio);
            }

            foreach (var ps in _fireCones)
            {
                if (ps == null)
                    continue;

                var emission = ps.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxFireEmission, fireRatio);

                if (fireRatio > 0.01f && !ps.isPlaying)
                    ps.Play();
                else if (fireRatio <= 0.01f && ps.isPlaying)
                    ps.Stop();
            }
        }

        private void HandleStretch()
        {
            if (_viewContainer == null || _isSquashing)
                return;

            Vector3 targetScale = new Vector3(
                _defaultScale.x * (2f - _stretchY),
                _defaultScale.y * _stretchY,
                _defaultScale.z);

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);
        }

        private void StartSquash()
        {
            if (_viewContainer == null)
                return;

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

            if (!string.IsNullOrEmpty(_activeLoopKey))
            {
                _audioService.StopSfx(_activeLoopKey);
                _activeLoopKey = null;
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            StopFlightEffects();

            _plungeDisposable?.Dispose();
            _impactDisposable?.Dispose();
        }
    }
}