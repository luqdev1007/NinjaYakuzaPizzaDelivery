using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;
using System;
using DG.Tweening;
using Assets._Project.Develop.Runtime.UI.Gameplay;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class PizzaHealthView : EntityView
    {
        [Header("Pizza Visual")]
        [SerializeField] private PizzaDisplayView _pizzaDisplay;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 0.5f;

        private IDisposable _damageEventDisposable;
        private Entity _linkedEntity;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);
            _pizzaDisplay.Initialize(entity);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _damageEventDisposable?.Dispose();

            if (_pizzaDisplay != null) 
                _pizzaDisplay.transform.DOKill();
        }

        private void OnDamaged(DamageData data)
        {
            if (_pizzaDisplay == null) 
                return;

            _pizzaDisplay.UpdateHealthVisual(_linkedEntity.CurrentHealth.Value, _linkedEntity.MaxHealth.Value);

            _pizzaDisplay.transform.DOComplete();
            _pizzaDisplay.transform.DOShakePosition(_shakeDuration, _shakeStrength);
        }
    }
}

