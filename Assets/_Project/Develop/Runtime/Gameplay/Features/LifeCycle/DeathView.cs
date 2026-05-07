using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    [RequireComponent(typeof(Animator))]
    public class DeathView : EntityView
    {
        private static readonly int IsDyingKey = Animator.StringToHash("IsDying");

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("Audio Settings")]
        [SerializeField] private SfxEvent _deathSoundConfig;

        private AudioService _audioService;
        private IDisposable _isDeadChangedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;

            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);

            if (entity.IsDead.Value)
                UpdateIsDead(true, false);
        }

        private void OnIsDeadChanged(bool old, bool isDead)
        {
            UpdateIsDead(isDead, isDead); 
        }

        private void UpdateIsDead(bool value, bool playSound)
        {
            if (_animator != null)
                _animator.SetBool(IsDyingKey, value);

            _audioService.HandleSFXEvent(_deathSoundConfig);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable?.Dispose();
        }
    }
}