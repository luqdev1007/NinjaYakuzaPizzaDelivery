using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;
using System;
using DG.Tweening;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageView : EntityView
    {
        [Header("Damage VFX")]
        [SerializeField] private ParticleSystem _applyDamageEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private ParticleSystemStopAction _vfxStopAction;

        [Header("Flash Effect")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.1f;

        [Header("Pizza Visual (Optional)")]
        [SerializeField] private PizzaDisplayView _pizzaDisplay;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 0.5f;

        [Header("Audio")]
        [SerializeField] private string _damageSoundPrefix = "HeroHit";
        [SerializeField] private bool _useVariation = true;

        private AudioService _audioService;
        private IDisposable _damageEventDisposable;
        private Entity _linkedEntity;
        private Color _originalColor;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);

            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;

            if (_pizzaDisplay != null)
                _pizzaDisplay.Initialize(entity);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _damageEventDisposable?.Dispose();
            if (_spriteRenderer != null) _spriteRenderer.DOKill();
        }

        private void OnDamaged(DamageData data)
        {
            SpawnDamageParticles();
            PlayFlashEffect();
            PlayDamageSound();

            if (_pizzaDisplay != null)
            {
                _pizzaDisplay.UpdateHealthVisual(_linkedEntity.CurrentHealth.Value, _linkedEntity.MaxHealth.Value);
                _pizzaDisplay.transform.DOComplete();
                _pizzaDisplay.transform.DOShakePosition(_shakeDuration, _shakeStrength);
            }
        }

        private void PlayDamageSound()
        {
            if (_useVariation)
                _audioService.PlaySfxVariation(_damageSoundPrefix, 1, 3, UnityEngine.Random.Range(0.9f, 1.1f));
            else
                _audioService.PlaySfxByPrefix(_damageSoundPrefix, true);
        }

        private void PlayFlashEffect()
        {
            if (_spriteRenderer == null) return;

            _spriteRenderer.DOKill();
            _spriteRenderer.color = _originalColor;

            _spriteRenderer.DOColor(_flashColor, _flashDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _spriteRenderer.color = _originalColor);
        }

        private void SpawnDamageParticles()
        {
            if (_applyDamageEffectPrefab == null) return;

            Vector3 spawnPos = _effectSpawnPoint != null ? _effectSpawnPoint.position : transform.position;
            ParticleSystem vfx = Instantiate(_applyDamageEffectPrefab, spawnPos, Quaternion.identity);
            var main = vfx.main;
            main.stopAction = (ParticleSystemStopAction)_vfxStopAction;
        }
    }
}