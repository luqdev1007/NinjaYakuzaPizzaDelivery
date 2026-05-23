using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; // Добавлено
using UnityEngine;
using System;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageView : EntityView, IRequireAudioService // Подключаем интерфейс
    {
        [Header("Damage VFX")]
        [SerializeField] private ParticleSystem _applyDamageEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private ParticleSystemStopAction _vfxStopAction;

        [Header("Flash Effect")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.1f;

        [Header("SFX Keys")] // Добавлено для настройки звука получения урона
        [SerializeField] private string _takeDamageSfxKey = "TakeDamage";

        private IDisposable _damageEventDisposable;
        private Color _originalColor;
        private IAudioService _audioService; // Ссылка на сервис звука

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);
            _originalColor = _spriteRenderer.color;
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
            PlayDamageSound(); // Триггерим звук
        }

        private void PlayFlashEffect()
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.color = _originalColor;

            _spriteRenderer.DOColor(_flashColor, _flashDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _spriteRenderer.color = _originalColor);
        }

        private void SpawnDamageParticles()
        {
            ParticleSystem vfx = Instantiate(_applyDamageEffectPrefab, _effectSpawnPoint.position, Quaternion.identity);

            var main = vfx.main;
            main.stopAction = _vfxStopAction;
        }

        private void PlayDamageSound()
        {
            // Воспроизводим 3D-звук в точке получения урона (на позиции вьюшки)
            _audioService?.PlaySfx(_takeDamageSfxKey, transform.position);
        }
    }
}