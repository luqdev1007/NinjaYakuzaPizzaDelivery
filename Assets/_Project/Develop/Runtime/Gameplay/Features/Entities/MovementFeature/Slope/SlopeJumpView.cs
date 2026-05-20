using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeJumpView : EntityView, IRequireAudioService
    {
        [Header("SFX Keys")]
        [SerializeField] private string _slopeJumpSfxKey = "AbilitySlopeJump";

        private IAudioService _audioService;
        private IDisposable _slopeJumpDisposable;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Подписываемся на ивент из системы
            _slopeJumpDisposable = entity.SlopeJumpEvent.Subscribe(OnSlopeJumpExecuted);
        }

        private void OnSlopeJumpExecuted(float speedFactor)
        {
            if (_audioService == null) return;

            // Воспроизводим звук в точке нахождения сущности
            _audioService.PlaySfx(_slopeJumpSfxKey, transform.position);

            // Если твой IAudioService поддерживает динамическое изменение параметров из кода, 
            // можно прокидывать перегрузку с модификатором:
            // _audioService.PlaySfx(_slopeJumpSfxKey, transform.position, speedFactor);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            // Чистим подписку во избежание утечек памяти
            _slopeJumpDisposable?.Dispose();
        }
    }
}