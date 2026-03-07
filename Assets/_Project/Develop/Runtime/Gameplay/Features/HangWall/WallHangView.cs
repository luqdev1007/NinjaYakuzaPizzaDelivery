using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    [RequireComponent(typeof(Animator))]
    public class WallHangView : EntityView
    {
        private readonly int IsWallHangingKey = Animator.StringToHash("IsWallHanging");
        [SerializeField] private Animator _animator;
        private IReadOnlyVariable<bool> _isWallHanging;
        private IDisposable _isWallHangingDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isWallHanging = entity.IsWallHanging;
            _isWallHangingDisposable = _isWallHanging.Subscribe(OnChanged);
            _animator.SetBool(IsWallHangingKey, _isWallHanging.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isWallHangingDisposable?.Dispose();
        }

        private void OnChanged(bool oldValue, bool value) =>
            _animator.SetBool(IsWallHangingKey, value);
    }
}
