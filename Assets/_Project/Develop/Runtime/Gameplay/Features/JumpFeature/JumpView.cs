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
        [SerializeField] private string _jumpSoundPrefix = "MainHeroJump";

        [Header("Double Jump Effects")]
        [SerializeField] private ParticleSystem _doubleJumpDustPS;
        [SerializeField] private string _doubleJumpSoundPrefix = "MainHeroJump"; // Можно использовать тот же префикс

        [Header("Landing Effects")]
        [SerializeField] private ParticleSystem _landDustPS;
        [SerializeField] private string _landSoundPrefix = "MainHeroLand";
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
            // Играем рандомную вариацию прыжка (Element 5 в конфиге)
            _audioService.PlaySfxByPrefixAuto(_jumpSoundPrefix, UnityEngine.Random.Range(0.95f, 1.15f));
        }

        private void OnDoubleJump()
        {
            _doubleJumpDustPS?.Play();
            // Для двойного прыжка можно чуть завысить питч для эффекта усиления
            _audioService.PlaySfxByPrefixAuto(_doubleJumpSoundPrefix, UnityEngine.Random.Range(1.2f, 1.3f));
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            // Если приземлились с большой скоростью
            if (value && !oldValue && _velocityYBeforeLand < _landVelocityThreshold)
            {
                _landDustPS?.Play();
                _audioService.PlaySfxByPrefixAuto(_landSoundPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
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