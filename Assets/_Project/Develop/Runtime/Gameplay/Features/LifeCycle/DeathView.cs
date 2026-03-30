using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    [RequireComponent(typeof(Animator))]
    public class DeathView : EntityView
    {
        private readonly int IsDyingKey = Animator.StringToHash("IsDying");

        [SerializeField] private Animator _animator;
        private IDisposable _isDeadChangedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
            if (entity.IsDead.Value) UpdateIsDead(true);
        }

        private void OnIsDeadChanged(bool old, bool isDead)
        {
            UpdateIsDead(isDead);
        }

        private void UpdateIsDead(bool value)
        {
            if (_animator != null)
                _animator.SetBool(IsDyingKey, value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDeadChangedDisposable?.Dispose();
        }
    }
}