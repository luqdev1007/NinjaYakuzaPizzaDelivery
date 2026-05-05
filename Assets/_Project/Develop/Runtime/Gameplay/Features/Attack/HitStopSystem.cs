using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class HitStopSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ICoroutinesPerformer _coroutines;
        private Entity _entity;
        private IDisposable _hitSubscription;

        public HitStopSystem(ICoroutinesPerformer coroutines)
        {
            _coroutines = coroutines;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _hitSubscription = _entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit);
        }

        private void OnSuccessfulHit()
        {
            if (_entity.AttackHitStopDuration.Value > 0)
            {
                _coroutines.StartPerform(DoHitStop());
            }
        }

        private IEnumerator DoHitStop()
        {
            float originalScale = Time.timeScale;
            Time.timeScale = _entity.AttackHitStopScale.Value;
            yield return new WaitForSecondsRealtime(_entity.AttackHitStopDuration.Value);
            Time.timeScale = originalScale;
        }

        public void OnDispose() => _hitSubscription?.Dispose();
    }
}