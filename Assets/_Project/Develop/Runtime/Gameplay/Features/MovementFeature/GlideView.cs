using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class GlideView : EntityView
    {
        private readonly int IsGlidingKey = Animator.StringToHash("IsGliding");

        [SerializeField] private Animator _animator;

        private IReadOnlyVariable<bool> _isGliding;
        private IDisposable _isGlidingDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGliding = entity.IsGliding;
            _isGlidingDisposable = _isGliding.Subscribe(OnIsGlidingChanged);
            UpdateIsGliding(_isGliding.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGlidingDisposable?.Dispose();
        }

        private void UpdateIsGliding(bool value) =>
            _animator.SetBool(IsGlidingKey, value);

        private void OnIsGlidingChanged(bool oldValue, bool value) =>
            UpdateIsGliding(value);
    }
}