using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleView : EntityView
    {
        private static readonly int IsThrowingHookKey = Animator.StringToHash("IsThrowingHook");

        [SerializeField] private Animator _animator;
        private IReadOnlyVariable<bool> _isThrowing;
        private IDisposable _disposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            // _isThrowing = entity.IsThrowing;

            // Исправлено: добавляем второй аргумент (старое значение), который игнорируем
            _disposable = _isThrowing.Subscribe((_, val) => _animator.SetBool(IsThrowingHookKey, val));

            // Синхронизируем начальное состояние
            _animator.SetBool(IsThrowingHookKey, _isThrowing.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _disposable?.Dispose();
        }
    }
}