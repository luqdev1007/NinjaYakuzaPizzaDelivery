using System;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpView : EntityView
    {
        [Header("Jump Effects")]
        [SerializeField] private ParticleSystem _jumpDustPrefab;
        [SerializeField] private string _jumpSoundPrefix = "MainHeroJump";

        [Header("Double Jump Effects")]
        [SerializeField] private ParticleSystem _doubleJumpDustPrefab;
        [SerializeField] private string _doubleJumpSoundPrefix = "MainHeroJump";

        [Header("Landing Effects")]
        [SerializeField] private ParticleSystem _landDustPrefab;
        [SerializeField] private string _landSoundPrefix = "MainHeroLand";
        [SerializeField] private float _landVelocityThreshold = -5f;

        private AudioService _audioService;
        private IVfxPoolService _vfxPool;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;

        private IDisposable _jumpDisposable;
        private IDisposable _doubleJumpDisposable;
        private IDisposable _groundedDisposable;
        private float _velocityYBeforeLand;

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
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
            SpawnVfx(_jumpDustPrefab);
            _audioService.PlaySfxByPrefixAuto(_jumpSoundPrefix, UnityEngine.Random.Range(0.95f, 1.15f));
        }

        private void OnDoubleJump()
        {
            SpawnVfx(_doubleJumpDustPrefab);
            _audioService.PlaySfxByPrefixAuto(_doubleJumpSoundPrefix, UnityEngine.Random.Range(1.2f, 1.3f));
        }

        private void OnGroundedChanged(bool oldValue, bool value)
        {
            if (value && !oldValue && _velocityYBeforeLand < _landVelocityThreshold)
            {
                SpawnVfx(_landDustPrefab);
                _audioService.PlaySfxByPrefixAuto(_landSoundPrefix, UnityEngine.Random.Range(0.9f, 1.1f));
            }
        }

        private void SpawnVfx(ParticleSystem prefab)
        {
            if (prefab != null && _vfxPool != null)
            {
                _vfxPool.Spawn(prefab, transform.position, Quaternion.identity);
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