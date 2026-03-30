using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
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
        [SerializeField] private string _startGlidePrefix = "GlideStart"; // GlideStart1, GlideStart2
        [SerializeField] private string _loopGlidePrefix = "GlideLoop";   // GlideLoop1, GlideLoop2, GlideLoop3

        [Header("Sway Settings")]
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _isGliding;
        private IDisposable _isGlidingDisposable;

        private string _activeLoopId; // Храним ID запущенного цикла, чтобы его остановить
        private bool _isCurrentlyGliding;
        private float _swayTimer;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Берем AudioService через твой стандартный AudioComponent
            _audioService = entity.GetComponent<AudioComponent>().Service;

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

            // 1. Аниматор
            if (_animator != null)
                _animator.SetBool(IsGlidingKey, isGliding);

            // 2. Частицы
            if (_airParticlesPS != null)
            {
                if (isGliding) _airParticlesPS.Play();
                else _airParticlesPS.Stop();
            }

            // 3. Звуковая логика через AudioService
            HandleAudio(isGliding);
        }

        // В GlideView.cs измени HandleAudio:

        private void HandleAudio(bool isGliding)
        {
            if (isGliding)
            {
                // 1. Собираем правильный префикс, как в инспекторе (AbilityImpact)
                // На скрине префикс именно такой.
                string startPrefix = "AbilityImpact" + _startGlidePrefix; // Получится "AbilityImpactGlideStart"
                string loopPrefix = "AbilityImpact" + _loopGlidePrefix;   // Получится "AbilityImpactGlideLoop"

                // 2. Разовый звук (вариации 1-2)
                _audioService.PlaySfxVariation(startPrefix, 1, 2, UnityEngine.Random.Range(0.95f, 1.05f));

                // 3. Цикл (вариации 1-3)
                // Метод PlaySfxVariationLoop внутри сам добавит индекс к префиксу и проверит конфиг
                _activeLoopId = _audioService.PlaySfxVariationLoop(loopPrefix, 1, 3);

                Debug.Log($"[GlideView] Trying to play loop with prefix: {loopPrefix}");
            }
            else
            {
                if (!string.IsNullOrEmpty(_activeLoopId))
                {
                    _audioService.StopSfx(_activeLoopId);
                    _activeLoopId = null;
                }
            }
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

            Quaternion currentRot = _viewContainer.localRotation;
            Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);

            _viewContainer.localRotation = Quaternion.Lerp(
                currentRot,
                targetRot,
                Time.deltaTime * _swayLerpSpeed
            );
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGlidingDisposable?.Dispose();

            // На всякий случай гасим звук при уничтожении
            if (!string.IsNullOrEmpty(_activeLoopId))
                _audioService.StopSfx(_activeLoopId);
        }
    }
}