using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HitStop;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Attack
{
    public class HitStopSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly HitStopService _hitStopService;
        private readonly CameraService _cameraService;

        private IDisposable _hitDisposable;

        private ReactiveVariable<float> _hitStopDuration;
        private ReactiveVariable<float> _hitStopScale;

        private float _nextAllowedHitStopTime;

        public HitStopSystem(HitStopService hitStopService, CameraService cameraService)
        {
            _hitStopService = hitStopService;
            _cameraService = cameraService;
        }

        public void OnInit(Entity entity)
        {
            _hitStopDuration = entity.AttackHitStopDuration;
            _hitStopScale = entity.AttackHitStopScale;

            _hitDisposable = entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit);
        }

        private void OnSuccessfulHit()
        {
            if (Time.unscaledTime < _nextAllowedHitStopTime)
                return;

            float duration = _hitStopDuration.Value;

            _hitStopService.PlayHitStop(duration, _hitStopScale.Value);
            _cameraService.GenerateHitShake(forceMultiplier: 100f);

            _nextAllowedHitStopTime = Time.unscaledTime + (duration * 1.2f);
        }

        public void OnDispose()
        {
            _hitDisposable?.Dispose();
        }
    }
}