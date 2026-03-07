using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    [RequireComponent(typeof(Animator))]
    public class GrappleView : EntityView
    {
        private readonly int IsThrowingHookKey = Animator.StringToHash("IsThrowingHook");

        [SerializeField] private Animator _animator;

        private ReactiveVariable<bool> _isThrowing;
        private IDisposable _isThrowingHookDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isThrowing = entity.IsThrowing;
            _isThrowingHookDisposable = _isThrowing.Subscribe(OnIsThrowingChanged);
            _animator.SetBool(IsThrowingHookKey, _isThrowing.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isThrowingHookDisposable?.Dispose();
        }

        private void OnIsThrowingChanged(bool oldValue, bool value) =>
            _animator.SetBool(IsThrowingHookKey, value);
    }
}

