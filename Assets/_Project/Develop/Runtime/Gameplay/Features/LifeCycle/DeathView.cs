using System;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    [RequireComponent(typeof(Animator))]
    public class DeathView : EntityView
    {
        private static readonly int IsDyingKey = Animator.StringToHash("IsDying");

        [Header("Components")]
        [SerializeField] private Animator _animator;

        [Header("Audio Settings")]
        [SerializeField] private string _deathSoundPrefix = "MainHeroDeath";

        private AudioService _audioService;
        private IDisposable _isDeadChangedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
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

            if (playSound && !string.IsNullOrEmpty(_deathSoundPrefix))
            {
                _audioService.PlaySfxByPrefixAuto(_deathSoundPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDeadChangedDisposable?.Dispose();
        }
    }
}