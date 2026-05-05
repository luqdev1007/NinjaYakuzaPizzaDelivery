using System;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class HeroWallJumpVfxView : EntityView
    {
        private static readonly int WallJumpTrigger = Animator.StringToHash("WallJump");

        [Header("Animation & Transform")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _rotationDuration = 0.45f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _jumpDustPrefab;
        [SerializeField] private Transform _dustSpawnPoint;

        [Header("Audio")]
        [SerializeField] private string _jumpSfxPrefix = "AbilityWallJump";

        private AudioService _audioService;
        private IVfxPoolService _vfxPool;
        private IDisposable _jumpSub;
        private Vector3 _baseScale;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _baseScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;

            _jumpSub = entity.IsWallJumping.Subscribe((_, isJumping) =>
            {
                if (isJumping)
                    ExecuteWallJumpJuice();
            });
        }

        private void ExecuteWallJumpJuice()
        {
            _audioService.PlaySfxByPrefixAuto(_jumpSfxPrefix, 1f);

            _animator.SetTrigger(WallJumpTrigger);

            if (_jumpDustPrefab != null && _vfxPool != null)
            {
                _vfxPool.Spawn(_jumpDustPrefab, _dustSpawnPoint.position, _dustSpawnPoint.rotation);
            }

            if (_viewContainer != null)
            {
                ApplyTweenJuice();
            }
        }

        private void ApplyTweenJuice()
        {
            _viewContainer.DOKill();
            _viewContainer.localScale = _baseScale;

            float direction = -Mathf.Sign(transform.localScale.x);

            _viewContainer.DOLocalRotate(new Vector3(0, 0, 360 * direction), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _viewContainer.localRotation = Quaternion.identity);

            _viewContainer.DOScale(new Vector3(_baseScale.x * 0.7f, _baseScale.y * 1.3f, 1f), _rotationDuration * 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _jumpSub?.Dispose();

            if (_viewContainer != null)
                _viewContainer.DOKill();
        }
    }
}