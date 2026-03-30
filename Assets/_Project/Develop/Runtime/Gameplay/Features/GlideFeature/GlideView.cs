using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideView : EntityView
    {
        private static readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [Header("VFX Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _airParticlesPS;
        [SerializeField] private Transform _viewContainer;

        [Header("Audio Settings")]
        [SerializeField] private string _startGlidePrefix = "GlideStart";
        [SerializeField] private string _loopGlidePrefix = "GlideLoop";
        [SerializeField] private string _endGlidePrefix = "GlideStart"; // Используем тот же префикс для закрытия или замени на свой

        [Header("Pitch Settings")]
        [SerializeField] private float _minPitch = 0.9f;
        [SerializeField] private float _maxPitch = 1.3f;
        [SerializeField] private float _minVelocityY = -15f; // Скорость быстрого падения
        [SerializeField] private float _maxVelocityY = -2f;  // Скорость медленного парения

        [Header("Sway Settings")]
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _isGliding;
        private Rigidbody2D _rigidbody;
        private IDisposable _isGlidingDisposable;

        private string _activeLoopId;
        private bool _isCurrentlyGliding;
        private float _swayTimer;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;

            _isGliding = entity.IsGliding;
            _rigidbody = entity.Rigidbody; // Предполагается, что в Entity есть реактивная переменная VelocityY

            _isGlidingDisposable = _isGliding.Subscribe(OnIsGlidingChanged);

            ApplyState(_isGliding.Value);
        }

        private void Update()
        {
            HandleSway();
            UpdateLoopPitch();
        }

        private void OnIsGlidingChanged(bool oldValue, bool newValue)
        {
            ApplyState(newValue);
        }

        private void ApplyState(bool isGliding)
        {
            _isCurrentlyGliding = isGliding;
            _swayTimer = 0f;

            if (_animator != null)
                _animator.SetBool(IsGlidingKey, isGliding);

            if (_airParticlesPS != null)
            {
                if (isGliding) _airParticlesPS.Play();
                else _airParticlesPS.Stop();
            }

            HandleAudio(isGliding);
        }

        private void HandleAudio(bool isGliding)
        {
            string startPrefix = "AbilityImpact" + _startGlidePrefix;
            string loopPrefix = "AbilityImpact" + _loopGlidePrefix;
            string endPrefix = "AbilityImpact" + _endGlidePrefix;

            if (isGliding)
            {
                // Звук открытия
                _audioService.PlaySfxVariation(startPrefix, 1, 2, UnityEngine.Random.Range(0.95f, 1.05f));
                // Запуск цикла
                _activeLoopId = _audioService.PlaySfxVariationLoop(loopPrefix, 1, 3);
            }
            else
            {
                // Если мы летели и перестали (сработал триггер выхода)
                if (!string.IsNullOrEmpty(_activeLoopId))
                {
                    _audioService.StopSfx(_activeLoopId);
                    _activeLoopId = null;

                    // Звук закрытия/конца глайда
                    _audioService.PlaySfxVariation(endPrefix, 1, 2, UnityEngine.Random.Range(0.85f, 0.95f));
                }
            }
        }

        private void UpdateLoopPitch()
        {
            if (!_isCurrentlyGliding || string.IsNullOrEmpty(_activeLoopId)) return;

            // Инвертируем VelocityY, так как падение — это отрицательные значения
            // Используем InverseLerp для получения значения от 0 до 1 между мин и макс скоростью
            float t = Mathf.InverseLerp(_maxVelocityY, _minVelocityY, _rigidbody.linearVelocityY);
            float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, t);

            // Обновляем питч у конкретного запущенного loop-звука
            _audioService.SetPitch(_activeLoopId, targetPitch);
        }

        private void HandleSway()
        {
            if (_viewContainer == null) return;

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

            if (!string.IsNullOrEmpty(_activeLoopId))
                _audioService.StopSfx(_activeLoopId);
        }
    }
}