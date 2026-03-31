using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

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

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
        }

        private void Update()
        {
            if (_rigidbody == null) 
                return;

            // Передаем скорость для плавных переходов Idle <-> Run и Jump <-> Fall
            _animator.SetFloat(VelocityXKey, Mathf.Abs(_rigidbody.linearVelocity.x));
            _animator.SetFloat(VelocityYKey, _rigidbody.linearVelocity.y);
            _animator.SetBool(IsGroundedKey, _isGrounded.Value);
        }
    }
}