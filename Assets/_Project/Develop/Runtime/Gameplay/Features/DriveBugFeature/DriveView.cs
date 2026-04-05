using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature
{
    public class DriveView : EntityView
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem _sparkPS;

        [Header("Audio Prefixes")]
        [SerializeField] private string _activationSfxPrefix = "DriveActivation"; // Высокочастотный дзынь
        [SerializeField] private string _loopSfxPrefix = "DriveLoop";             // Глитчевый гул

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _isDriveActive;
        private IDisposable _disposable;
        private string _activeLoopId;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _isDriveActive = entity.IsDriveActive;
            _disposable = _isDriveActive.Subscribe(OnDriveChanged);
        }

        private void OnDriveChanged(bool oldVal, bool isActive)
        {
            if (isActive)
            {
                _sparkPS?.Play();

                // 1. Звук активации
                _audioService.PlaySfxByPrefixAuto(_activationSfxPrefix, 1.2f);

                // 2. Запуск цикла
                _activeLoopId = _audioService.PlaySfxVariationLoop(_loopSfxPrefix, 1, 1);
            }
            else
            {
                _sparkPS?.Stop();

                // Останавливаем цикл
                if (!string.IsNullOrEmpty(_activeLoopId))
                {
                    _audioService.StopSfx(_activeLoopId);
                    _activeLoopId = null;
                }
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _disposable?.Dispose();
            if (!string.IsNullOrEmpty(_activeLoopId)) _audioService.StopSfx(_activeLoopId);
        }
    }
}