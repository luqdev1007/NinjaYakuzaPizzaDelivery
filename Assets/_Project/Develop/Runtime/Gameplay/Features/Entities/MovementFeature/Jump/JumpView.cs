using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; 

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
{
    public class JumpView : EntityView, IRequireAudioService
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _jumpDustPS;
        [SerializeField] private ParticleSystem _landDustPS;
        [SerializeField] private float _landVelocityThreshold = -5f;

        [Header("SFX Keys")]
        [SerializeField] private string _jumpSfxKey = "Jump";
        [SerializeField] private string _landSfxKey = "Land";

        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;

        private IDisposable _jumpDisposable;
        private IDisposable _groundedDisposable;

        private IAudioService _audioService; 

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

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

            _audioService?.PlaySfx(_jumpSfxKey, transform.position);
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            if (value && !oldValue && _rigidbody.linearVelocity.y < _landVelocityThreshold)
            {
                _landDustPS?.Play();
                _audioService?.PlaySfx(_landSfxKey, transform.position);
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