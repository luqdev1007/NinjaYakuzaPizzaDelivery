using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; // Не забудь про пространство звуков

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature
{
    [RequireComponent(typeof(Animator))]
    public class SpawnProcessView : EntityView, IRequireAudioService
    {
        private static readonly int SpawningProcessKey = Animator.StringToHash("IsSpawning");

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("VFX Settings")]
        [SerializeField] private ParticleSystem _spawnEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private ParticleSystemStopAction _vfxStopAction = ParticleSystemStopAction.Destroy;

        [Header("SFX Settings (Optional)")]
        [SerializeField] private string _spawnSfxKey = "LootSpawnPopup"; // Твой ключ звука спавна

        private IReadOnlyVariable<bool> _inSpawnProcess;
        private IDisposable _inSpawnProcessChangedDisposable;
        private IAudioService _audioService;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
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
            if (_animator != null)
            {
                _animator.SetBool(SpawningProcessKey, value);
            }

            if (value)
            {
                PlaySpawnEffect();
                PlaySpawnAudio();
            }
        }

        private void PlaySpawnEffect()
        {
            if (_spawnEffectPrefab == null) return;

            Transform spawnPoint = _effectSpawnPoint != null ? _effectSpawnPoint : transform;
            ParticleSystem vfx = Instantiate(_spawnEffectPrefab, spawnPoint.position, spawnPoint.rotation);

            var main = vfx.main;
            main.stopAction = _vfxStopAction;

            vfx.Play();
        }

        private void PlaySpawnAudio()
        {
            if (!string.IsNullOrEmpty(_spawnSfxKey))
            {
                _audioService?.PlaySfx(_spawnSfxKey, transform.position);
            }
        }
    }
}