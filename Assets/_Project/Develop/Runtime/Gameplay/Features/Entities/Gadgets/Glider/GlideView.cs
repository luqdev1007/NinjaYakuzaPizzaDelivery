using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideView : EntityView, IRequireAudioService
    {
        private static readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [Header("VFX Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _airParticlesPS;
        [SerializeField] private Transform _viewContainer;

        [Header("Audio Settings")]
        [SerializeField] private string _startGlidePrefix = "GlideStart";
        [SerializeField] private string _loopGlidePrefix = "GlideLoop";
        [SerializeField] private string _endGlidePrefix = "GlideEnd"; // Поменял дефолт на GlideEnd в соответствии с ассетом

        [Header("Sway Settings")]
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private IAudioService _audioService;
        private IReadOnlyVariable<bool> _isGliding;
        private IDisposable _isGlidingDisposable;

        private bool _isCurrentlyGliding;
        private float _swayTimer;
        private string _cachedLoopKey;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

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
            if (_audioService == null) return;

            string startKey = _startGlidePrefix;
            string loopKey = _loopGlidePrefix;
            string endKey = _endGlidePrefix;

            if (isGliding)
            {
                _audioService.PlaySfx(startKey);
                _audioService.PlaySfxLoop(loopKey);
                _cachedLoopKey = loopKey;
            }
            else
            {
                if (!string.IsNullOrEmpty(_cachedLoopKey))
                {
                    _audioService.StopSfx(_cachedLoopKey);
                    _cachedLoopKey = null;
                    _audioService.PlaySfx(endKey);
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

            if (!string.IsNullOrEmpty(_cachedLoopKey))
            {
                _audioService?.StopSfx(_cachedLoopKey);
                _cachedLoopKey = null;
            }
        }
    }
}