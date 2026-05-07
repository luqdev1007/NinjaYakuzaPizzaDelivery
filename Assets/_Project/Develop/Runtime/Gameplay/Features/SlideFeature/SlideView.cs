using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideView : EntityView
    {
        private static readonly int IsSlidingKey = Animator.StringToHash("IsSliding");

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("VFX - Friction & Dust")]
        [SerializeField] private ParticleSystem _frictionPS;
        [SerializeField] private float _maxEmissionRate = 50f;
        [SerializeField] private float _vfxRampUpTime = 0.3f;

        [Header("Audio Settings")]
        [SerializeField] private SfxEvent _slideLoopSoundConfig; 
        [SerializeField] private float _minPitch = 0.9f;
        [SerializeField] private float _maxPitch = 1.3f;

        [Header("Visual Deformation")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _slideStretchX = 1.3f;
        [SerializeField] private float _slideSquashY = 0.7f;
        [SerializeField] private float _slideTiltAngle = 12f;
        [SerializeField] private float _lerpSpeed = 10f;
        [SerializeField] private float _slopeRotationLerp = 0.15f;

        private AudioService _audioService;
        private SlopeSystem _slopeSystem;
        private IReadOnlyVariable<bool> _isSliding;
        private IReadOnlyVariable<bool> _isOnSlope;

        private SfxEvent _activeLoopEvent; 
        private float _slideTimer;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;
        private IDisposable _slideDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _slopeSystem = entity.GetSystem<SlopeSystem>();
            _isSliding = entity.IsSliding;
            _isOnSlope = entity.IsOnSlope;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;
            _defaultRotation = _viewContainer != null ? _viewContainer.localRotation : Quaternion.identity;

            _slideDisposable = _isSliding.Subscribe(OnSlideChanged);
            _animator.SetBool(IsSlidingKey, _isSliding.Value);
        }

        private void Update()
        {
            HandleRotationAndDeformation();

            if (!_isSliding.Value) return;

            _slideTimer += Time.deltaTime;
            float intensity = Mathf.Clamp01(_slideTimer / _vfxRampUpTime);

            UpdateVFX(intensity);
            UpdateAudio(intensity);
        }

        private void HandleRotationAndDeformation()
        {
            if (_viewContainer == null) return;

            Vector3 targetScale = _defaultScale;
            if (_isSliding.Value)
            {
                targetScale = new Vector3(_defaultScale.x * _slideStretchX, _defaultScale.y * _slideSquashY, _defaultScale.z);
            }
            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);

            float targetZ = 0f;

            if (_isSliding.Value)
            {
                if (_isOnSlope.Value && _slopeSystem != null)
                {
                    targetZ = Vector2.SignedAngle(Vector2.up, _slopeSystem.SlopeNormal);
                    float direction = Mathf.Sign(transform.localScale.x);
                    targetZ += (direction > 0 ? -90f : 90f);
                }
                else
                {
                    float direction = Mathf.Sign(transform.localScale.x);
                    targetZ = -_slideTiltAngle * direction;
                }
            }

            Quaternion targetRot = Quaternion.Euler(0, 0, targetZ);
            _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRot, _slopeRotationLerp);
        }

        private void OnSlideChanged(bool oldValue, bool isSliding)
        {
            _animator.SetBool(IsSlidingKey, isSliding);

            if (isSliding)
            {
                _slideTimer = 0f;
                if (_frictionPS != null) 
                    _frictionPS.Play();

                _audioService.PlayLoopEvent(_slideLoopSoundConfig);
                _activeLoopEvent = _slideLoopSoundConfig;
            }
            else
            {
                StopEffects();
            }
        }

        private void UpdateVFX(float intensity)
        {
            if (_frictionPS == null) return;
            var emission = _frictionPS.emission;
            emission.rateOverTime = Mathf.Lerp(5f, _maxEmissionRate, intensity);
        }

        private void UpdateAudio(float intensity)
        {
            if (_activeLoopEvent == null) 
                return;

            float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, intensity);
            _audioService.SetLoopPitch(_activeLoopEvent, targetPitch);
        }

        private void StopEffects()
        {
            if (_frictionPS != null) _frictionPS.Stop();

            if (_activeLoopEvent != null)
            {
                _audioService.StopLoopEvent(_activeLoopEvent);
                _activeLoopEvent = null;
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            StopEffects();
            _slideDisposable?.Dispose();
            if (_viewContainer != null)
            {
                _viewContainer.localScale = _defaultScale;
                _viewContainer.localRotation = _defaultRotation;
            }
        }
    }
}