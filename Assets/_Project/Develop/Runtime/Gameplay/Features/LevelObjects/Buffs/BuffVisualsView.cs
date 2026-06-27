using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffVisualsView : EntityView, IRequireAudioService
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _pickupVfxPrefab;

        [Header("SFX")]
        [SerializeField] private string _pickupSfxKey = "BuffPickup";

        private IReadOnlyVariable<bool> _isCollected;
        private IDisposable _isCollectedDisposable;
        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isCollected = entity.BuffIsCollected;
            _isCollectedDisposable = _isCollected.Subscribe(OnCollectedStateChanged);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isCollectedDisposable?.Dispose();
        }

        private void OnCollectedStateChanged(bool oldVal, bool isCollected)
        {
            if (isCollected)
            {
                if (_pickupVfxPrefab != null)
                {
                    Instantiate(_pickupVfxPrefab, transform.position, transform.rotation);
                }

                if (!string.IsNullOrEmpty(_pickupSfxKey))
                {
                    _audioService?.PlaySfx(_pickupSfxKey, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}