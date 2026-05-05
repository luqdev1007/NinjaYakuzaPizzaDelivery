using System;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Loot
{
    public class SecretChestAnimatedView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _openAnimName = "IsOpened";

        [Header("Audio")]
        [SerializeField] private string _openSoundPrefix = "ChestOpen";

        private AudioService _audioService;
        private IDisposable _healthDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _healthDisposable = entity.CurrentHealth.Subscribe((oldHp, newHp) =>
            {
                if (newHp <= 0 && oldHp > 0)
                {
                    OpenChest();
                }
            });
        }

        private void OpenChest()
        {
            if (_animator != null)
                _animator.SetBool(_openAnimName, true);

            if (!string.IsNullOrEmpty(_openSoundPrefix))
                _audioService?.PlaySfxByPrefixAuto(_openSoundPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _healthDisposable?.Dispose();
        }
    }
}