using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    [RequireComponent(typeof(Animator))]
    public class SlideView : EntityView
    {
        private readonly int IsSlidingKey = Animator.StringToHash("IsSliding");
        [SerializeField] private Animator _animator;
        private IReadOnlyVariable<bool> _isSliding;
        private IDisposable _isSlidingDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isSliding = entity.IsSliding;
            _isSlidingDisposable = _isSliding.Subscribe(OnChanged);
            _animator.SetBool(IsSlidingKey, _isSliding.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isSlidingDisposable?.Dispose();
        }

        private void OnChanged(bool oldValue, bool value) =>
            _animator.SetBool(IsSlidingKey, value);
    }
}
