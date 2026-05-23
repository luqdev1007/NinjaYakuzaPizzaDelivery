using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.HitStop;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Attack
{
    public class HitStopSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly HitStopService _hitStopService;

        private IDisposable _hitDisposable;

        private ReactiveVariable<float> _hitStopDuration;
        private ReactiveVariable<float> _hitStopScale;

        public HitStopSystem(HitStopService hitStopService)
        {
            _hitStopService = hitStopService;
        }

        public void OnInit(Entity entity)
        {
            _hitStopDuration = entity.AttackHitStopDuration;
            _hitStopScale = entity.AttackHitStopScale;

            _hitDisposable = entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit);
        }

        private void OnSuccessfulHit()
        {
            _hitStopService.PlayHitStop(_hitStopDuration.Value, _hitStopScale.Value);
        }

        public void OnDispose()
        {
            _hitDisposable?.Dispose();
        }
    }
}