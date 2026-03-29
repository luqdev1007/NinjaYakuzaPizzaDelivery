using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using UnityEngine;
using System;
using DG.Tweening;

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

        private ReactiveEvent<DamageData> _damageEvent;
        private IDisposable _damageEventDisposable;
        private Entity _linkedEntity;
        private Color _originalColor;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _damageEvent = entity.TakeDamageEvent;
            _damageEventDisposable = _damageEvent.Subscribe(OnDamaged);

            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;

            if (_pizzaDisplay != null)
            {
                _pizzaDisplay.Initialize(entity);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _damageEventDisposable?.Dispose();
            _spriteRenderer.DOKill();
        }

        private void OnDamaged(DamageData data)
        {
            SpawnDamageParticles();
            PlayFlashEffect();

            if (_pizzaDisplay != null)
            {
                _pizzaDisplay.UpdateHealthVisual(_linkedEntity.CurrentHealth.Value, _linkedEntity.MaxHealth.Value);

                _pizzaDisplay.transform.DOComplete();
                _pizzaDisplay.transform.DOShakePosition(_shakeDuration, _shakeStrength);
            }
        }

        private void PlayFlashEffect()
        {
            if (_spriteRenderer == null) return;

            _spriteRenderer.DOKill();
            _spriteRenderer.color = _originalColor;

            // Короткий блик: красим в белый и возвращаем обратно
            _spriteRenderer.DOColor(_flashColor, _flashDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _spriteRenderer.color = _originalColor);
        }

        private void SpawnDamageParticles()
        {
            if (_applyDamageEffectPrefab == null)
                return;

            Vector3 spawnPos = _effectSpawnPoint != null ? _effectSpawnPoint.position : transform.position;
            ParticleSystem vfx = Instantiate(_applyDamageEffectPrefab, spawnPos, Quaternion.identity);
            ParticleSystem.MainModule main = vfx.main;
            main.stopAction = _vfxStopAction;
        }
    }
}