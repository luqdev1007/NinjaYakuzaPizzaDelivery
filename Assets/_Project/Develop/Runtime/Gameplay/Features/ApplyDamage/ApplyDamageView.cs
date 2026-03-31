using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
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

        [Header("Audio")]
        [Tooltip("Для героя: MainHeroHit. Для призрака: GhostHit")]
        [SerializeField] private string _damageSoundPrefix = "MainHeroHit";

        private AudioService _audioService;
        private IDisposable _damageEventDisposable;
        private Color _originalColor;

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Получаем сервис аудио через компонент сущности
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);

            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
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

            // 1. Формируем специфический префикс (например, GhostHit + Shuriken)
            string typeSuffix = data.Type == DamageType.Cut ? "Shuriken" : data.Type.ToString();
            string specificPrefix = _damageSoundPrefix + typeSuffix;

            // 2. Логика Fallback:
            // Сначала ищем специфический звук (например, GhostHitShuriken1)
            if (_audioService.GetVariationCount(specificPrefix) > 0)
            {
                _audioService.PlaySfxByPrefixAuto(specificPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
            }
            // Если его нет, ищем базовый звук (например, GhostHit1, GhostHit2)
            else if (_audioService.GetVariationCount(_damageSoundPrefix) > 0)
            {
                _audioService.PlaySfxByPrefixAuto(_damageSoundPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
            }
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
            main.stopAction = _vfxStopAction;
        }
    }
}