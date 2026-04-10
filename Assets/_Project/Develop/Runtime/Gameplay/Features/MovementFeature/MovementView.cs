using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class MovementView : EntityView
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        [SerializeField, Min(0.1f)] private float _maxSpeedMultiplier = 2f;

        [Header("VFX - Dust")]
        [SerializeField] private ParticleSystem _runDustPS;
        [SerializeField] private float _runDustSpeedThreshold = 2f;
        [SerializeField] private ParticleSystem _brakeDustPS;
        [SerializeField] private float _brakeSpeedThreshold = 4f;
        [SerializeField] private float _brakeDirectionThreshold = 0.5f;
        [SerializeField] private ParticleSystem _startDustPS;
        [SerializeField] private float _startSpeedThreshold = 1f;

        [Header("Audio")]
        [SerializeField] private string _footstepPrefix = "MainHeroFootstep";
        [SerializeField] private float _baseFootstepInterval = 0.35f;

        private readonly int IsRunningKey = Animator.StringToHash("IsRunning");
        private readonly int RunSpeedMultiplierKey = Animator.StringToHash("RunAnimationSpeedMultiplier");

        private AudioService _audioService;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<bool> _isMoving;

        private IReadOnlyVariable<bool> _isOnSlope;
        private IReadOnlyVariable<bool> _isDashing;
        private IReadOnlyVariable<bool> _isSliding;

        private IDisposable _isMovingDisposable;

        private float _maxSpeed;
        private float _previousVelocityX;
        private bool _wasMoving;
        private float _footstepTimer;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isMoving = entity.IsMoving;
            _maxSpeed = entity.MoveSpeed.Value;

            _isOnSlope = entity.IsOnSlope;
            _isDashing = entity.IsDashing;
            _isSliding = entity.IsSliding;

            _wasMoving = _isMoving.Value;

            // Подписка на изменение состояния движения
            _isMovingDisposable = _isMoving.Subscribe((oldValue, newValue) =>
            {
                _animator.SetBool(IsRunningKey, newValue);
            });

            _animator.SetBool(IsRunningKey, _isMoving.Value);
        }

        private void Update()
        {
            if (_rigidbody == null) return;

            float velocityX = _rigidbody.linearVelocity.x;
            bool grounded = _isGrounded.Value;
            bool moving = _isMoving.Value;

            // Вычисляем интенсивность бега
            float speedRatio = Mathf.Clamp01(Mathf.Abs(velocityX) / _maxSpeed);

            UpdateAnimationSpeed(speedRatio);
            UpdateRunVFXAndAudio(grounded, moving, speedRatio);
            UpdateBrakeVFX(grounded, velocityX);
            UpdateStartMoveVFX(grounded, moving, velocityX);

            _previousVelocityX = velocityX;
            _wasMoving = moving;
        }

        private void UpdateAnimationSpeed(float speedRatio)
        {
            float multiplier = Mathf.Lerp(1f, _maxSpeedMultiplier, speedRatio);
            _animator.SetFloat(RunSpeedMultiplierKey, multiplier);
        }

        private void UpdateRunVFXAndAudio(bool grounded, bool moving, float speedRatio)
        {
            bool isFastEnough = Mathf.Abs(_rigidbody.linearVelocity.x) > _runDustSpeedThreshold;
            bool isRunning = grounded && moving && isFastEnough;

            // Частицы бега
            if (_runDustPS != null)
            {
                if (isRunning && !_runDustPS.isPlaying) _runDustPS.Play();
                else if (!isRunning && _runDustPS.isPlaying) _runDustPS.Stop();
            }

            // Звук шагов
            if (isRunning && _isDashing.Value == false && _isSliding.Value == false && _isOnSlope.Value == false)
            {
                float currentMultiplier = Mathf.Lerp(1f, _maxSpeedMultiplier, speedRatio);
                _footstepTimer -= Time.deltaTime * currentMultiplier;

                if (_footstepTimer <= 0f)
                {
                    _audioService.PlaySfxByPrefixAuto(_footstepPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
                    _footstepTimer = _baseFootstepInterval;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }

        private void UpdateBrakeVFX(bool grounded, float velocityX)
        {
            if (!grounded || _brakeDustPS == null) return;

            // Проверка резкой смены направления
            bool changingDirection =
                (_previousVelocityX > _brakeSpeedThreshold && velocityX < -_brakeDirectionThreshold) ||
                (_previousVelocityX < -_brakeSpeedThreshold && velocityX > _brakeDirectionThreshold);

            // Проверка резкой остановки
            bool hardStop =
                Mathf.Abs(_previousVelocityX) > _brakeSpeedThreshold &&
                Mathf.Abs(velocityX) < _brakeDirectionThreshold;

            if (changingDirection || hardStop)
            {
                _brakeDustPS.Play();
            }
        }

        private void UpdateStartMoveVFX(bool grounded, bool moving, float velocityX)
        {
            if (_startDustPS == null) return;

            bool justStartedMoving = grounded && moving && !_wasMoving &&
                Mathf.Abs(velocityX) > _startSpeedThreshold;

            if (justStartedMoving)
            {
                _startDustPS.Play();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isMovingDisposable?.Dispose();
            _rigidbody = null;
        }
    }
}