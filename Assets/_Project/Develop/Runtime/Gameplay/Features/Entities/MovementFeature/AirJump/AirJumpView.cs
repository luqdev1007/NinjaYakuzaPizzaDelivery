using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; 
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump
{
    public class AirJumpView : EntityView, IRequireAudioService 
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _airJumpVFX;
        [SerializeField] private ParticleSystem _canAirJumpVFX;

        [Header("SFX Keys")]
        [SerializeField] private string _airJumpSfxKey = "AirJump"; 

        private ICompositeCondition _canAirJump;
        private IDisposable _airJumpDisposable;

        private IAudioService _audioService; 

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _airJumpDisposable = entity.AirJumpEvent.Subscribe(OnAirJump);
            _canAirJump = entity.CanAirJump;
        }

        private void OnAirJump()
        {
            _airJumpVFX?.Play();

            _audioService?.PlaySfx(_airJumpSfxKey, transform.position);

            // Debug.Log("Air jump vfx and sfx event!");
        }

        private void Update()
        {
            if (_canAirJump == null || _canAirJumpVFX == null) 
                return;

            ParticleSystem.EmissionModule emission = _canAirJumpVFX.emission;
            emission.enabled = _canAirJump.Evaluate();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _airJumpDisposable?.Dispose();
        }
    }
}