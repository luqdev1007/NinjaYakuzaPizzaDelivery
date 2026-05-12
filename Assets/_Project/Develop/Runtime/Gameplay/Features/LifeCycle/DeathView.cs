using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
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

        private IDisposable _isDeadChangedDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        private void OnIsDeadChanged(bool old, bool isDead)
        {
            _animator.SetBool(IsDyingKey, isDead);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable?.Dispose();
        }
    }
}