using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideView : EntityView
    {
        private static readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [Header("VFX Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _airLines;
        [SerializeField] private Transform _viewContainer;

        [Header("Audio Settings")]
        [SerializeField] private string _startGlidePrefix = "GlideStart";
        [SerializeField] private string _loopGlidePrefix = "GlideLoop";
        [SerializeField] private string _endGlidePrefix = "GlideEnd";

        [Header("Pitch Settings")]
        [SerializeField] private float _minPitch = 0.9f;
        [SerializeField] private float _maxPitch = 1.3f;
        [SerializeField] private float _minVelocityY = -15f;
        [SerializeField] private float _maxVelocityY = -2f;

        [Header("Sway Settings")]
        [SerializeField] private float _swayAngle = 5f;
        [SerializeField] private float _swaySpeed = 2f;
        [SerializeField] private float _swayLerpSpeed = 5f;

        private AudioService _audioService;
        private Rigidbody2D _rigidbody;
        private IDisposable _isGlidingSub;

        private string _activeLoopId;
        private bool _isCurrentlyGliding;
        private float _swayTimer;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;

            _isGlidingSub = entity.IsGliding.Subscribe(OnIsGlidingChanged);

            ApplyState(entity.IsGliding.Value);
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

            if (_airLines != null)
            {
                if (isGliding) _airLines.Play();
                else _airLines.Stop();
            }

            HandleAudio(isGliding);
        }

        private void HandleAudio(bool isGliding)
        {
            if (_audioService == null) return;

            string startPrefix = "AbilityImpact" + _startGlidePrefix;
            string loopPrefix = "AbilityImpact" + _loopGlidePrefix;
            string endPrefix = "AbilityImpact" + _endGlidePrefix;

            if (isGliding)
            {
                _audioService.PlaySfxVariation(startPrefix, 1, 2, UnityEngine.Random.Range(0.95f, 1.05f));
                _activeLoopId = _audioService.PlaySfxVariationLoop(loopPrefix, 1, 3);
            }
            else
            {
                if (!string.IsNullOrEmpty(_activeLoopId))
                {
                    _audioService.StopSfx(_activeLoopId);
                    _activeLoopId = null;

                    _audioService.PlaySfxVariation(endPrefix, 1, 2, UnityEngine.Random.Range(0.85f, 0.95f));
                }
            }
        }

        private void UpdateLoopPitch()
        {
            if (!_isCurrentlyGliding || string.IsNullOrEmpty(_activeLoopId) || _rigidbody == null)
                return;

            float t = Mathf.InverseLerp(_maxVelocityY, _minVelocityY, _rigidbody.linearVelocityY);
            float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, t);

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
            _isGlidingSub?.Dispose();

            if (!string.IsNullOrEmpty(_activeLoopId) && _audioService != null)
            {
                _audioService.StopSfx(_activeLoopId);
                _activeLoopId = null;
            }
        }
    }
}