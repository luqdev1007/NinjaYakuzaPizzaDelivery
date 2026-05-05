using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature
{
    public class HeroSpawnVfxView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        private static readonly int SpawningProcessKey = Animator.StringToHash("IsSpawning");

        [Header("VFX")]
        [SerializeField] private ParticleSystem _spawnEffectPrefab;
        [SerializeField] private Transform _spawnPoint;

        [Header("Audio")]
        [SerializeField] private string _soundPrefix = "LifeCycleSpawn";

        private AudioService _audioService;
        private IVfxPoolService _vfxPool;
        private IDisposable _spawnSub;

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _spawnSub = entity.InSpawnProcess.Subscribe((_, isInProcess) =>
            {
                _animator.SetBool(SpawningProcessKey, isInProcess);

                if (isInProcess)
                    ExecuteSpawnFeedback();
            });

            _animator.SetBool(SpawningProcessKey, entity.InSpawnProcess.Value);
        }

        private void ExecuteSpawnFeedback()
        {
            PlayEffect();

            if (_audioService != null)
            {
                _audioService.PlaySfxByPrefixAuto(_soundPrefix, UnityEngine.Random.Range(0.95f, 1.05f));
            }
        }

        private void PlayEffect()
        {
            if (_spawnEffectPrefab != null && _vfxPool != null)
            {
                var pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
                var rot = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

                _vfxPool.Spawn(_spawnEffectPrefab, pos, rot);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _spawnSub?.Dispose();
        }
    }
}