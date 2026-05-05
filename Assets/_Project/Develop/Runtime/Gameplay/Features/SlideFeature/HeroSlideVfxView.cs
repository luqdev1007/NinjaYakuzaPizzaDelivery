using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class HeroSlideVfxView : EntityView
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        private static readonly int IsSlidingKey = Animator.StringToHash("IsSliding");

        [Header("VFX")]
        [SerializeField] private ParticleSystem _frictionPS;
        [SerializeField] private float _maxEmissionRate = 50f;
        [SerializeField] private float _vfxRampUpTime = 0.3f;

        [Header("Audio")]
        [SerializeField] private string _slideLoopPrefix = "AbilityImpactSlide";
        [SerializeField] private float _minPitch = 0.9f;
        [SerializeField] private float _maxPitch = 1.3f;

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _isSliding;
        private IDisposable _slideSub;
        private string _activeLoopId;
        private float _slideTimer;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _isSliding = entity.IsSliding;

            _slideSub = _isSliding.Subscribe((_, isSliding) => {
                _animator.SetBool(IsSlidingKey, isSliding);
                if (isSliding) StartEffects(); else StopEffects();
            });
        }

        private void Update()
        {
            if (!_isSliding.Value) return;

            _slideTimer += Time.deltaTime;
            float intensity = Mathf.Clamp01(_slideTimer / _vfxRampUpTime);

            UpdateVfx(intensity);
            UpdateAudio(intensity);
        }

        private void UpdateVfx(float intensity)
        {
            if (_frictionPS == null) return;
            var emission = _frictionPS.emission;
            emission.rateOverTime = Mathf.Lerp(5f, _maxEmissionRate, intensity);
        }

        private void UpdateAudio(float intensity)
        {
            if (string.IsNullOrEmpty(_activeLoopId)) return;
            _audioService.SetPitch(_activeLoopId, Mathf.Lerp(_minPitch, _maxPitch, intensity));
        }

        private void StartEffects()
        {
            _slideTimer = 0f;
            _frictionPS?.Play();
            _activeLoopId = _audioService.PlaySfxVariationLoop(_slideLoopPrefix, 1, 3);
        }

        private void StopEffects()
        {
            _frictionPS?.Stop();
            if (!string.IsNullOrEmpty(_activeLoopId)) _audioService.StopSfx(_activeLoopId);
            _activeLoopId = null;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            StopEffects();
            _slideSub?.Dispose();
        }
    }
}