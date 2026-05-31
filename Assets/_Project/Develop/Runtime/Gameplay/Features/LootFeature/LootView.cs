using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootView : EntityView, IRequireAudioService
    {
        [SerializeField] private string _soundKey = "CollectSound";

        private IAudioService _audioService;
        private IDisposable _collectDisposable;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _collectDisposable = entity.LootIsCollected.Subscribe(OnCollectChanged);
        }

        private void OnCollectChanged(bool oldValue, bool isCollected)
        {
            if (isCollected)
            {
                PlayCollectEffects();
            }
        }

        private void PlayCollectEffects()
        {
            if (!string.IsNullOrEmpty(_soundKey))
            {
                _audioService?.PlaySfx(_soundKey, transform.position);
            }

            // _pickupParticle.Play();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _collectDisposable?.Dispose();
        }
    }
}