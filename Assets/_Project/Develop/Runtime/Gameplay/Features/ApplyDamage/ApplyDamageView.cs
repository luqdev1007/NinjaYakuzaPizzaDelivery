using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Infrastructure.DI;
using UnityEngine;
using System;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageView : EntityView
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _damageVfxPrefab;
        [SerializeField] private Transform _vfxSpawnPoint;

        [Header("Flash Effect")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.1f;

        private AudioService _audioService;
        private IVfxPoolService _vfxPool;
        private IDisposable _damageSub;
        private Color _originalColor;

        private static readonly string HitSfxCut = "MainHeroHitShuriken";
        private static readonly string HitSfxGeneric = "MainHeroHitGeneric";

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _damageSub = entity.TakeDamageEvent.Subscribe(OnDamaged);

            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _damageSub?.Dispose();

            if (_spriteRenderer != null)
                _spriteRenderer.DOKill();
        }

        private void OnDamaged(DamageData data)
        {
            PlayVfx();
            PlayFlashEffect();
            PlayHitSfx(data.Type);
        }

        private void PlayVfx()
        {
            if (_damageVfxPrefab != null && _vfxPool != null)
            {
                Vector3 pos = _vfxSpawnPoint != null ? _vfxSpawnPoint.position : transform.position;
                _vfxPool.Spawn(_damageVfxPrefab, pos, Quaternion.identity);
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

        private void PlayHitSfx(DamageType type)
        {
            if (_audioService == null) return;

            string sfxKey = type == DamageType.Cut ? HitSfxCut : HitSfxGeneric;

            _audioService.PlaySfxByPrefixAuto(sfxKey, UnityEngine.Random.Range(0.9f, 1.1f));
        }
    }
}