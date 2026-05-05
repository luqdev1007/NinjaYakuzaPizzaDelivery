using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class HeroFootstepsView : EntityView
    {
        [SerializeField] private string _stepPrefix = "MainHeroFootstep";
        [SerializeField] private float _baseInterval = 0.35f;

        private AudioService _audioService;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<bool> _isMoving;
        private float _timer;

        protected override void OnDependencyResolve(DIContainer container)
            => _audioService = container.Resolve<AudioService>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isMoving = entity.IsMoving;
        }

        private void Update()
        {
            if (_rigidbody == null || !_isGrounded.Value || !_isMoving.Value) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _audioService.PlaySfxByPrefixAuto(_stepPrefix, Random.Range(0.9f, 1.1f));
                _timer = _baseInterval;
            }
        }
    }
}