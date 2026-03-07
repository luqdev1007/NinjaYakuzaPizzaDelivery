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

        private IReadOnlyVariable<bool> _isThrowingHook;
        private IDisposable _isThrowingHookDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isThrowingHook = entity.IsThrowingHook;
            _isThrowingHookDisposable = _isThrowingHook.Subscribe(OnIsThrowingHookChanged);
            _animator.SetBool(IsThrowingHookKey, _isThrowingHook.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isThrowingHookDisposable?.Dispose();
        }

        private void OnIsThrowingHookChanged(bool oldValue, bool value) =>
            _animator.SetBool(IsThrowingHookKey, value);
    }
}

