using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    [RequireComponent(typeof(Animator))]
    public class PlungeView : EntityView
    {
        private readonly int IsPlungingKey = Animator.StringToHash("IsPlunging");
        [SerializeField] private Animator _animator;
        private IReadOnlyVariable<bool> _isPlunging;
        private IDisposable _isPlungingDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isPlunging = entity.IsPlunging;
            _isPlungingDisposable = _isPlunging.Subscribe(OnChanged);
            _animator.SetBool(IsPlungingKey, _isPlunging.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isPlungingDisposable?.Dispose();
        }

        private void OnChanged(bool oldValue, bool value) =>
            _animator.SetBool(IsPlungingKey, value);
    }
}
