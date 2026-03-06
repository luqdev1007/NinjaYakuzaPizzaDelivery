using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class DashView : EntityView
    {
        private readonly int IsDashingKey = Animator.StringToHash("IsDashing");

        [SerializeField] private Animator _animator;

        private IReadOnlyVariable<bool> _isDashing;
        private IDisposable _isDashingDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDashing = entity.IsDashing;
            _isDashingDisposable = _isDashing.Subscribe(OnIsDashingChanged);
            UpdateIsDashing(_isDashing.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDashingDisposable?.Dispose();
        }

        private void UpdateIsDashing(bool value) =>
            _animator.SetBool(IsDashingKey, value);

        private void OnIsDashingChanged(bool oldValue, bool value) =>
            UpdateIsDashing(value);
    }
}