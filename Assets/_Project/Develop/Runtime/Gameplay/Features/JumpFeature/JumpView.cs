using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpView : EntityView
    {
        [SerializeField] private ParticleSystem _jumpDustPS;
        [SerializeField] private ParticleSystem _landDustPS;
        [SerializeField] private float _landVelocityThreshold = -5f;

        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;

        private IDisposable _jumpDisposable;
        private IDisposable _groundedDisposable;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;

            _jumpDisposable = entity.JumpEvent.Subscribe(OnJump);
            _groundedDisposable = _isGrounded.Subscribe(OnGroundedChanged);
        }

        private void OnJump()
        {
            _jumpDustPS?.Play();
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            if (value && !oldValue && _rigidbody.linearVelocity.y < _landVelocityThreshold)
            {
                _landDustPS?.Play();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _jumpDisposable?.Dispose();
            _groundedDisposable?.Dispose();
        }
    }
}