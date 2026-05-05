using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Infrastructure.DI;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackAnimationClip;

        [Header("VFX Prefabs (From Pool)")]
        [Tooltip("Префабы слэшей, которые будут запрашиваться у пула")]
        [SerializeField] private ParticleSystem[] _slashPrefabs;
        [SerializeField] private Transform _slashSpawnPoint;

        [Header("Audio")]
        [SerializeField] private string _swingPrefix = "SwordSwing";
        [SerializeField] private string _hitPrefix = "EnemyHit";

        private AudioService _audioService;
        private IVfxPoolService _vfxPool;

        private int _currentSlashIndex;
        private Transform _entityTransform;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entityTransform = entity.Transform;

            if (_attackAnimationClip != null && entity.HasComponent<AttackProcessInitialTime>())
            {
                float speedMultiplier = _attackAnimationClip.length / entity.AttackProcessInitialTime.Value;
                _animator.SetFloat(SpeedMultiplierKey, speedMultiplier);
            }

            _inAttackProcessDisposable = entity.InAttackProcess.Subscribe(OnAttackProcessChanged);
            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(OnAttackMoment);

            if (entity.HasComponent<SuccessfulHitEvent>())
                _successfulHitDisposable = entity.GetComponent<SuccessfulHitEvent>().Value.Subscribe(OnSuccessfulHit);
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            if (current)
            {
                _animator.SetTrigger(AttackTrigger);

                if (_audioService != null)
                    _audioService.PlaySfxByPrefixAuto(_swingPrefix, UnityEngine.Random.Range(0.95f, 1.05f));
            }
        }

        private void OnAttackMoment() => SpawnSlashVfx();

        private void OnSuccessfulHit()
        {
            if (_audioService != null)
                _audioService.PlaySfxByPrefixAuto(_hitPrefix, UnityEngine.Random.Range(0.95f, 1.1f));
        }

        private void SpawnSlashVfx()
        {
            if (_slashPrefabs == null || _slashPrefabs.Length == 0 || _vfxPool == null) 
                return;

            ParticleSystem prefab = _slashPrefabs[_currentSlashIndex];

            if (prefab != null)
            {
                Vector3 spawnPos = _slashSpawnPoint != null ? _slashSpawnPoint.position : transform.position;

                ParticleSystem effect = _vfxPool.Spawn(prefab, spawnPos, Quaternion.identity);

                if (_entityTransform != null)
                {
                    Vector3 scale = effect.transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * (_entityTransform.localScale.x > 0 ? 1f : -1f);
                    effect.transform.localScale = scale;
                }
            }

            _currentSlashIndex = (_currentSlashIndex + 1) % _slashPrefabs.Length;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _inAttackProcessDisposable?.Dispose();
            _attackHitDisposable?.Dispose();
            _successfulHitDisposable?.Dispose();
        }
    }
}