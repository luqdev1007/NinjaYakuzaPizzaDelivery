using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class JumpView : EntityView
    {
        private readonly int IsGroundedKey = Animator.StringToHash("IsGrounded");
        private readonly int VelocityYKey = Animator.StringToHash("VelocityY");

        [SerializeField] private Animator _animator;

        private IReadOnlyVariable<bool> _isGrounded;
        private IDisposable _isGroundedDisposable;
        private Rigidbody2D _rigidbody;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _rigidbody = entity.Rigidbody;

            _isGroundedDisposable = _isGrounded.Subscribe(OnIsGroundedChanged);
            UpdateIsGrounded(_isGrounded.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGroundedDisposable?.Dispose();
        }

        private void Update()
        {
            if (_rigidbody == null) 
                return;

            _animator.SetFloat(VelocityYKey, _rigidbody.linearVelocity.y);
        }

        private void UpdateIsGrounded(bool value) =>
            _animator.SetBool(IsGroundedKey, value);

        private void OnIsGroundedChanged(bool oldValue, bool value) =>
            UpdateIsGrounded(value);
    }
}