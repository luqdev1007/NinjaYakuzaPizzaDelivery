using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature
{
    [RequireComponent(typeof(Animator))]
    public class SpawnProcessView : EntityView
    {
        private readonly int SpawningProcessKey = Animator.StringToHash("IsSpawning");

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("VFX Settings")]
        [SerializeField] private ParticleSystem _spawnEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private ParticleSystemStopAction _vfxStopAction = ParticleSystemStopAction.Destroy;

        [Header("Audio Settings")]
        [Tooltip("Префикс звука (например, LifeCycleSpawn). Система сама найдет вариации 1, 2, 3...")]
        [SerializeField] private string _spawnSoundPrefix = "LifeCycleSpawn";

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _inSpawnProcess;
        private IDisposable _inSpawnProcessChangedDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _inSpawnProcess = entity.InSpawnProcess;

            _inSpawnProcessChangedDisposable = _inSpawnProcess.Subscribe(OnSpawnProcessChanged);

            UpdateSpawnProcessState(_inSpawnProcess.Value);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _inSpawnProcessChangedDisposable?.Dispose();
        }

        private void OnSpawnProcessChanged(bool oldValue, bool newValue)
        {
            UpdateSpawnProcessState(newValue);
        }

        private void UpdateSpawnProcessState(bool value)
        {
            _animator.SetBool(SpawningProcessKey, value);

            if (value)
            {
                PlaySpawnEffect();
                PlaySpawnAudio();
            }
        }

        private void PlaySpawnEffect()
        {
            if (_spawnEffectPrefab == null) return;

            Vector3 position = _effectSpawnPoint != null ? _effectSpawnPoint.position : transform.position;
            Quaternion rotation = _effectSpawnPoint != null ? _effectSpawnPoint.rotation : Quaternion.identity;

            ParticleSystem vfx = Instantiate(_spawnEffectPrefab, position, rotation);
            var main = vfx.main;
            main.stopAction = _vfxStopAction;
            vfx.Play();
        }

        private void PlaySpawnAudio()
        {
            _audioService.PlaySfxByPrefixAuto(_spawnSoundPrefix, 1f);
        }
    }
}