using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    [RequireComponent(typeof(Animator))]
    public class RunAnimationSpeedView : EntityView
    {
        private readonly int _runSpeedMultiplierKey =
            Animator.StringToHash("RunAnimationSpeedMultiplier");

        [SerializeField] private Animator _animator;
        [SerializeField, Min(0.1f)] private float _maxSpeedMultiplier = 2f;

        private Rigidbody2D _rigidbody;
        private float _maxSpeed;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _maxSpeed = entity.MoveSpeed.Value;
        }

        private void Update()
        {
            if (_rigidbody == null) return;

            float speedRatio = Mathf.Abs(_rigidbody.linearVelocity.x) / _maxSpeed;
            float multiplier = Mathf.Lerp(1f, _maxSpeedMultiplier, speedRatio);
            _animator.SetFloat(_runSpeedMultiplierKey, multiplier);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _rigidbody = null;
        }
    }
}
