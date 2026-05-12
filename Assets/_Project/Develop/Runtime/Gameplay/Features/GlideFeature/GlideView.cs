using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideView : EntityView
    {
        private static readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [Header("VFX")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _airParticlesPS;
        [SerializeField] private Transform _viewContainer;

        [Header("Audio Settings")]
        [SerializeField] private SfxEvent _glideStartSoundConfig;
        [SerializeField] private SfxEvent _glideLoopSoundConfig;
        [SerializeField] private SfxEvent _glideEndSoundConfig;

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
        private IReadOnlyVariable<bool> _isGliding;
        private Rigidbody2D _rigidbody;
        private IDisposable _isGlidingDisposable;

        private bool _isCurrentlyGliding;
        private float _swayTimer;
        private SfxEvent _activeLoopEvent;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            
            /*
            _isGliding = entity.IsGliding;
            _rigidbody = entity.Rigidbody;
            */

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
            if (isGliding)
            {
                _audioService.HandleSFXEvent(_glideStartSoundConfig);

                _audioService.PlayLoopEvent(_glideLoopSoundConfig);
                _activeLoopEvent = _glideLoopSoundConfig;
            }
            else
            {
                if (_activeLoopEvent != null)
                {
                    _audioService.StopLoopEvent(_activeLoopEvent);
                    _activeLoopEvent = null;

                    _audioService.HandleSFXEvent(_glideEndSoundConfig);
                }
            }
        }

        private void UpdateLoopPitch()
        {
            if (!_isCurrentlyGliding || _activeLoopEvent == null) return;

            // Расчет питча: чем быстрее падаем (ниже значение Y), тем выше питч ветра
            float t = Mathf.InverseLerp(_maxVelocityY, _minVelocityY, _rigidbody.linearVelocityY);
            float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, t);

            _audioService.SetLoopPitch(_activeLoopEvent, targetPitch);
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

            if (_activeLoopEvent != null)
            {
                _audioService.StopLoopEvent(_activeLoopEvent);
                _activeLoopEvent = null;
            }
        }
    }
}