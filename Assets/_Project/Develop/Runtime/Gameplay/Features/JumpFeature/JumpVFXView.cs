using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpVFXView : EntityView
    {
        [Header("Jump")]
        [SerializeField] private ParticleSystem _jumpDustPS;

        [Header("Double Jump")]
        [SerializeField] private ParticleSystem _doubleJumpDustPS;

        [Header("Land")]
        [SerializeField] private ParticleSystem _landDustPS;
        [SerializeField] private float _landVelocityThreshold = -5f;

        private IReadOnlyVariable<bool> _isGrounded;
        private Rigidbody2D _rigidbody;

        private IDisposable _jumpDisposable;
        private IDisposable _doubleJumpDisposable;
        private IDisposable _groundedDisposable;

        private bool _wasGrounded;
        private float _velocityYBeforeLand;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;

            _wasGrounded = _isGrounded.Value;

            _jumpDisposable = entity.JumpEvent.Subscribe(OnJump);
            _doubleJumpDisposable = entity.DoubleJumpEvent.Subscribe(OnDoubleJump);
            _groundedDisposable = _isGrounded.Subscribe(OnGroundedChanged);
        }

        private void Update()
        {
            if (_rigidbody != null)
                _velocityYBeforeLand = _rigidbody.linearVelocity.y;
        }

        private void OnJump()
        {
            if (_jumpDustPS != null)
                _jumpDustPS.Play();
        }

        private void OnDoubleJump()
        {
            if (_doubleJumpDustPS != null)
                _doubleJumpDustPS.Play();
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            if (value && !oldValue && _velocityYBeforeLand < _landVelocityThreshold)
            {
                if (_landDustPS != null)
                    _landDustPS.Play();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _jumpDisposable?.Dispose();
            _doubleJumpDisposable?.Dispose();
            _groundedDisposable?.Dispose();
            _rigidbody = null;
        }
    }
}