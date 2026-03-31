using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeView : EntityView
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        private readonly int IsPlungingKey = Animator.StringToHash("IsPlunging");

        [Header("VFX - Flight (Ramp up)")]
        [SerializeField] private ParticleSystem _airConePS;
        [SerializeField] private ParticleSystem[] _fireCones;
        [Tooltip("Время до достижения максимальной силы эффектов")]
        [SerializeField] private float _fullPowerTime = 0.5f;
        [SerializeField] private float _maxAirEmission = 40f;
        [SerializeField] private float _maxFireEmission = 30f;

        [Header("VFX - Impact")]
        [SerializeField] private ParticleSystem _impactPS;

        [Header("Audio Settings")]
        [SerializeField] private string _plungeLoopPrefix = "AbilityImpactPlungeLoop";
        [SerializeField] private string _plungeLandPrefix = "AbilityImpactPlunge";
        [SerializeField] private float _minPitch = 1f;
        [SerializeField] private float _maxPitch = 1.6f;

        [Header("Squash & Stretch")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _stretchY = 1.4f;
        [SerializeField] private float _squashY = 0.6f;
        [SerializeField] private float _squashDuration = 0.15f;
        [SerializeField] private float _lerpSpeed = 12f;

        private AudioService _audioService;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isPlunging;
        private IReadOnlyVariable<bool> _isGrounded;

        private string _activeLoopId;
        private float _flightTimer;
        private bool _isSquashing;
        private Vector3 _defaultScale;

        private IDisposable _plungeDisposable;
        private IDisposable _groundedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rigidbody = entity.Rigidbody;
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;

            _plungeDisposable = _isPlunging.Subscribe(OnPlungeChanged);
            _groundedDisposable = _isGrounded.Subscribe(OnGroundedChanged);

            // Синхронизируем начальное состояние
            _animator.SetBool(IsPlungingKey, _isPlunging.Value);
        }

        private void Update()
        {
            if (!_isPlunging.Value) return;

            // Считаем прогресс полета
            _flightTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(_flightTimer / _fullPowerTime);

            UpdateVFXPower(ratio);
            UpdateSfxPower(ratio);
            HandleStretch();
        }

        private void OnPlungeChanged(bool oldValue, bool isPlunging)
        {
            _animator.SetBool(IsPlungingKey, isPlunging);

            if (isPlunging)
            {
                _flightTimer = 0f;
                _airConePS?.Play();

                // Запуск зацикленного звука (используем твой метод из AudioService)
                _activeLoopId = _audioService.PlaySfxVariationLoop(_plungeLoopPrefix, 1, 3);

                if (!string.IsNullOrEmpty(_activeLoopId))
                    _audioService.SetPitch(_activeLoopId, _minPitch);
            }
            else
            {
                StopFlightEffects();
            }
        }

        private void OnGroundedChanged(bool oldValue, bool grounded)
        {
            // Если приземлились и до этого летели в пике
            if (grounded && _flightTimer > 0.05f)
            {
                _impactPS?.Play();

                // Звук удара (чуть выше питч для сочности)
                _audioService.PlaySfxVariation(_plungeLandPrefix, 1, 3, 1.3f);

                StartSquash();
                StopFlightEffects();
            }
        }

        private void UpdateVFXPower(float ratio)
        {
            // Воздушный конус плавно усиливается с нуля
            if (_airConePS != null)
            {
                var emission = _airConePS.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxAirEmission, ratio);
            }

            // Огонь начинает появляться только после 40% прогресса полета
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

        private void UpdateSfxPower(float ratio)
        {
            if (string.IsNullOrEmpty(_activeLoopId)) return;

            // Питч звука растет вместе с визуалом
            float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, ratio);
            _audioService.SetPitch(_activeLoopId, targetPitch);
        }

        private void HandleStretch()
        {
            if (_viewContainer == null || _isSquashing) return;

            // Вытягиваем по вертикали, сужаем по горизонтали
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
            // Сплющиваем при ударе
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
                // Плавно возвращаемся в норму
                _viewContainer.localScale = Vector3.Lerp(squashScale, _defaultScale, t);
                yield return null;
            }

            _viewContainer.localScale = _defaultScale;
            _isSquashing = false;
        }

        private void StopFlightEffects()
        {
            _airConePS?.Stop();
            foreach (var ps in _fireCones) ps?.Stop();

            if (!string.IsNullOrEmpty(_activeLoopId))
            {
                // ИСПОЛЬЗУЕМ ТВОЙ МЕТОД StopSfx
                _audioService.StopSfx(_activeLoopId);
                _activeLoopId = null;
            }

            _flightTimer = 0f;
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