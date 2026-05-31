using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Props
{
    public class PropVisualsView : EntityView, IRequireAudioService
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _breakVfxPrefab;

        [Header("SFX")]
        [SerializeField] private string _breakSfxKey = "WoodPropBreak";
        [SerializeField] private string _hitSfxKey = "WoodPropHit";

        private IReadOnlyVariable<bool> _isDead;
        private ReactiveEvent<DamageData> _damageEvent;

        private IDisposable _isDeadDisposable;
        private IDisposable _damageDisposable;
        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDead = entity.IsDead;
            _damageEvent = entity.TakeDamageEvent;

            _isDeadDisposable = _isDead.Subscribe(OnDeathStateChanged);
            _damageDisposable = _damageEvent.Subscribe(OnDamageReceived);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDeadDisposable?.Dispose();
            _damageDisposable?.Dispose();
        }

        private void OnDamageReceived(DamageData damage)
        {
            if (_isDead.Value == false && !string.IsNullOrEmpty(_hitSfxKey))
            {
                _audioService?.PlaySfx(_hitSfxKey, transform.position);
            }
        }

        private void OnDeathStateChanged(bool oldVal, bool isDead)
        {
            if (isDead)
            {
                if (_breakVfxPrefab != null)
                {
                    Instantiate(_breakVfxPrefab, transform.position, transform.rotation);
                }

                if (!string.IsNullOrEmpty(_breakSfxKey))
                {
                    _audioService?.PlaySfx(_breakSfxKey, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}