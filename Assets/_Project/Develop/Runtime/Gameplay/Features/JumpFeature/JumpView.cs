using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpView : EntityView
    {
        [Header("Jump Effects")]
        [SerializeField] private ParticleSystem _jumpDustPS;
        [SerializeField] private SfxEvent _jumpSoundConfig;

        [Header("Double Jump Effects")]
        [SerializeField] private ParticleSystem _doubleJumpDustPS;
        [SerializeField] private SfxEvent _doubleJumpSoundConfig;

        [Header("Landing Effects")]
        [SerializeField] private ParticleSystem _landDustPS;
        [SerializeField] private SfxEvent _landSoundConfig;
        [SerializeField] private float _landVelocityThreshold = -5f;

        private AudioService _audioService;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;

        private IDisposable _jumpDisposable;
        private IDisposable _doubleJumpDisposable;
        private IDisposable _groundedDisposable;
        private float _velocityYBeforeLand;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;

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
            _jumpDustPS?.Play();

            _audioService.HandleSFXEvent(_jumpSoundConfig);
        }

        private void OnDoubleJump()
        {
            _doubleJumpDustPS?.Play();

            _audioService.HandleSFXEvent(_doubleJumpSoundConfig);
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            if (value && !oldValue && _velocityYBeforeLand < _landVelocityThreshold)
            {
                _landDustPS?.Play();
                _audioService.HandleSFXEvent(_landSoundConfig);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _jumpDisposable?.Dispose();
            _doubleJumpDisposable?.Dispose();
            _groundedDisposable?.Dispose();
        }
    }
}