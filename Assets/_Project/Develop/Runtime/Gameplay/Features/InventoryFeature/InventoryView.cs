using System;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InventoryFeature
{
    public class InventoryView : EntityView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _switchItemSfxPrefix = "ItemSwitch";
        [SerializeField] private string _throwSfxPrefix = "AbilityImpactHeroThrow";

        private static readonly int ThrowTrigger = Animator.StringToHash("Throw");

        private AudioService _audioService;
        private IDisposable _throwSubscription;
        private IDisposable _switchSubscription;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _throwSubscription = entity.ThrowEvent.Subscribe(OnThrow);
            _switchSubscription = entity.CurrentThrowableIndex.Subscribe((oldIdx, newIdx) => OnItemSwitched());
        }

        private void OnThrow()
        {
            if (_animator != null)
                _animator.SetTrigger(ThrowTrigger);

            _audioService?.PlaySfxByPrefixAuto(_throwSfxPrefix, Random.Range(0.9f, 1.1f));
        }

        private void OnItemSwitched()
        {
            _audioService?.PlaySfxByPrefixAuto(_switchItemSfxPrefix, Random.Range(0.95f, 1.05f));
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _throwSubscription?.Dispose();
            _switchSubscription?.Dispose();
        }
    }
}