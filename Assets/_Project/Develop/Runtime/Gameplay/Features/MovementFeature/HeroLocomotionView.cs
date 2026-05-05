using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class HeroLocomotionView : EntityView
    {
        [Header("Animator Settings")]
        [SerializeField] private Animator _animator;
        [SerializeField, Min(0.1f)] private float _maxSpeedMultiplier = 2f;

        private static readonly int IsRunningKey = Animator.StringToHash("IsRunning");
        private static readonly int RunSpeedMultKey = Animator.StringToHash("RunAnimationSpeedMultiplier");

        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isMoving;
        private IDisposable _movingSubscription;
        private float _maxSpeed;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isMoving = entity.IsMoving;
            _maxSpeed = entity.MoveSpeed.Value;

            _movingSubscription = _isMoving.Subscribe((_, moving) => _animator.SetBool(IsRunningKey, moving));
            _animator.SetBool(IsRunningKey, _isMoving.Value);
        }

        private void Update()
        {
            if (_rigidbody == null) 
                return;

            float speedRatio = Mathf.Clamp01(Mathf.Abs(_rigidbody.linearVelocity.x) / _maxSpeed);
            float multiplier = Mathf.Lerp(1f, _maxSpeedMultiplier, speedRatio);
            _animator.SetFloat(RunSpeedMultKey, multiplier);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _movingSubscription?.Dispose();
        }
    }
}