using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; // Добавлено
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move
{
    [RequireComponent(typeof(Animator))]
    public class MovementView : EntityView, IRequireAudioService // Подключаем интерфейс
    {
        private static readonly int IsRunningKey = Animator.StringToHash("IsRunning");
        private static readonly int RunSpeedMultiplierKey = Animator.StringToHash("RunAnimationSpeedMultiplier");

        [Header("Animator")]
        [SerializeField] private Animator _animator;
        [SerializeField, Min(0.1f)] private float _maxSpeedMultiplier = 2f;

        [Header("VFX - Run Dust")]
        [SerializeField] private ParticleSystem _runDustPS;
        [SerializeField] private float _runDustSpeedThreshold = 2f;

        [Header("VFX - Brake Dust")]
        [SerializeField] private ParticleSystem _brakeDustPS;
        [SerializeField] private float _brakeSpeedThreshold = 4f;
        [SerializeField] private float _brakeDirectionThreshold = 0.5f;

        [Header("VFX - Start Dust")]
        [SerializeField] private ParticleSystem _startDustPS;
        [SerializeField] private float _startSpeedThreshold = 1f;

        [Header("SFX Keys")]
        [SerializeField] private string _footstepSfxKey = "Footstep";
        [SerializeField] private string _brakeSfxKey = "Brake";
        [SerializeField] private string _startMoveSfxKey = "StartMove";

        private Rigidbody2D _rigidbody;
        private IAudioService _audioService; 

        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<bool> _isMoving;
        private IReadOnlyVariable<float> _maxSpeed;

        private IDisposable _isMovingDisposable;

        private float _previousVelocityX;
        private bool _wasMoving;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isMoving = entity.IsMoving;
            _maxSpeed = entity.MoveSpeed;

            _isMovingDisposable = _isMoving.Subscribe((_, newValue) =>
            {
                _animator.SetBool(IsRunningKey, newValue);
            });

            _animator.SetBool(IsRunningKey, _isMoving.Value);
            _wasMoving = _isMoving.Value;
        }

        private void Update()
        {
            if (_rigidbody == null || _isGrounded == null)
                return;

            float velocityX = _rigidbody.linearVelocity.x;
            bool grounded = _isGrounded.Value;
            bool moving = _isMoving.Value;

            UpdateAnimationSpeed(velocityX);

            UpdateRunVFX(grounded, moving, velocityX);
            UpdateBrakeVFX(grounded, velocityX);
            UpdateStartMoveVFX(grounded, moving, velocityX);

            _previousVelocityX = velocityX;
            _wasMoving = moving;
        }

        private void UpdateAnimationSpeed(float velocityX)
        {
            float speedRatio = Mathf.Clamp01(Mathf.Abs(velocityX) / _maxSpeed.Value);
            float multiplier = Mathf.Lerp(1f, _maxSpeedMultiplier, speedRatio);

            _animator.SetFloat(RunSpeedMultiplierKey, multiplier);
        }

        private void UpdateRunVFX(bool grounded, bool moving, float velocityX)
        {
            bool isFastEnough = Mathf.Abs(velocityX) > _runDustSpeedThreshold;
            bool shouldPlay = grounded && moving && isFastEnough;

            ToggleParticleSystem(_runDustPS, shouldPlay);
        }

        private void UpdateBrakeVFX(bool grounded, float velocityX)
        {
            if (!grounded) return;

            bool changingDirection =
                _previousVelocityX > _brakeSpeedThreshold && velocityX < -_brakeDirectionThreshold ||
                _previousVelocityX < -_brakeSpeedThreshold && velocityX > _brakeDirectionThreshold;

            bool hardStop =
                Mathf.Abs(_previousVelocityX) > _brakeSpeedThreshold &&
                Mathf.Abs(velocityX) < _brakeDirectionThreshold;

            if (changingDirection || hardStop)
            {
                _brakeDustPS.Play();

                _audioService?.PlaySfx(_brakeSfxKey, transform.position);
            }
        }

        private void UpdateStartMoveVFX(bool grounded, bool moving, float velocityX)
        {
            bool justStartedMoving = grounded && moving && !_wasMoving &&
                                     Mathf.Abs(velocityX) > _startSpeedThreshold;

            if (justStartedMoving)
            {
                _startDustPS.Play();

                _audioService?.PlaySfx(_startMoveSfxKey, transform.position);
            }
        }

        /// <summary>
        /// Animation Events
        /// </summary>
        public void PlayFootstep()
        {
            if (_isGrounded != null && _isGrounded.Value)
            {
                _audioService?.PlaySfx(_footstepSfxKey, transform.position);
            }
        }

        private void ToggleParticleSystem(ParticleSystem ps, bool shouldPlay)
        {
            if (shouldPlay && !ps.isPlaying)
                ps.Play();
            else if (!shouldPlay && ps.isPlaying)
                ps.Stop();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isMovingDisposable?.Dispose();
            _rigidbody = null;
        }
    }
}