using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RunVFXView : EntityView
    {
        [Header("Dust - Run")]
        [SerializeField] private ParticleSystem _runDustPS;
        [SerializeField] private float _runDustSpeedThreshold = 2f;

        [Header("Dust - Brake")]
        [SerializeField] private ParticleSystem _brakeDustPS;
        [SerializeField] private float _brakeSpeedThreshold = 4f;
        [SerializeField] private float _brakeDirectionThreshold = 0.5f;

        [Header("Dust - Start")]
        [SerializeField] private ParticleSystem _startDustPS;
        [SerializeField] private float _startSpeedThreshold = 1f;

        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<bool> _isMoving;

        private float _previousVelocityX;
        private bool _wasGrounded;
        private bool _wasMoving;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isMoving = entity.IsMoving;

            _wasGrounded = _isGrounded.Value;
            _wasMoving = _isMoving.Value;
        }

        private void Update()
        {
            if (_rigidbody == null)
                return;

            float velocityX = _rigidbody.linearVelocity.x;
            bool grounded = _isGrounded.Value;
            bool moving = _isMoving.Value;

            UpdateRunDust(grounded, moving);
            UpdateBrakeDust(grounded, velocityX);
            UpdateStartDust(grounded, moving);

            _previousVelocityX = velocityX;
            _wasGrounded = grounded;
            _wasMoving = moving;
        }

        private void UpdateRunDust(bool grounded, bool moving)
        {
            if (_runDustPS == null)
                return;

            bool shouldPlay = grounded && moving &&
                Mathf.Abs(_rigidbody.linearVelocity.x) > _runDustSpeedThreshold;

            if (shouldPlay && !_runDustPS.isPlaying)
                _runDustPS.Play();
            else if (!shouldPlay && _runDustPS.isPlaying)
                _runDustPS.Stop();
        }

        private void UpdateBrakeDust(bool grounded, float velocityX)
        {
            if (_brakeDustPS == null || !grounded)
                return;

            bool changingDirection =
                (_previousVelocityX > _brakeSpeedThreshold && velocityX < -_brakeDirectionThreshold) ||
                (_previousVelocityX < -_brakeSpeedThreshold && velocityX > _brakeDirectionThreshold);

            bool hardStop =
                Mathf.Abs(_previousVelocityX) > _brakeSpeedThreshold &&
                Mathf.Abs(velocityX) < _brakeDirectionThreshold;

            if (changingDirection || hardStop)
                _brakeDustPS.Play();
        }

        private void UpdateStartDust(bool grounded, bool moving)
        {
            if (_startDustPS == null)
                return;

            bool justStartedMoving = grounded && moving && !_wasMoving &&
                Mathf.Abs(_rigidbody.linearVelocity.x) > _startSpeedThreshold;

            if (justStartedMoving)
                _startDustPS.Play();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _rigidbody = null;
        }
    }
}