using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroAnimatorView : EntityView
    {
        private static readonly int VelocityXKey = Animator.StringToHash("VelocityX");
        private static readonly int VelocityYKey = Animator.StringToHash("VelocityY");
        private static readonly int IsGroundedKey = Animator.StringToHash("IsGrounded");

        [SerializeField] private Animator _animator;

        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;
        private bool _isActive;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive || _rigidbody == null) return;

            _animator.SetFloat(VelocityXKey, Mathf.Abs(_rigidbody.linearVelocity.x));
            _animator.SetFloat(VelocityYKey, _rigidbody.linearVelocity.y);
            _animator.SetBool(IsGroundedKey, _isGrounded.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isActive = false;
        }
    }
}