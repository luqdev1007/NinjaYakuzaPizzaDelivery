using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.UI.Gameplay; // Путь к твоей PizzaDisplayView
using UnityEngine;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageView : EntityView
    {
        [Header("Damage VFX")]
        [SerializeField] private ParticleSystem _applyDamageEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        [SerializeField] private ParticleSystemStopAction _vfxStopAction;

        [Header("Pizza Visual")]
        [SerializeField] private PizzaDisplayView _pizzaDisplay;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 0.5f;

        private ReactiveEvent<float> _damageEvent;
        private IDisposable _damageEventDisposable;
        private Entity _linkedEntity;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
            _damageEvent = entity.TakeDamageEvent;
            _damageEventDisposable = _damageEvent.Subscribe(OnDamaged);

            if (_pizzaDisplay != null)
            {
                _pizzaDisplay.Initialize(entity);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _damageEventDisposable?.Dispose();
        }

        private void OnDamaged(float value)
        {
            SpawnDamageParticles();
            if (_pizzaDisplay != null)
            {
                _pizzaDisplay.UpdateHealthVisual(_linkedEntity.CurrentHealth.Value, _linkedEntity.MaxHealth.Value);

                _pizzaDisplay.transform.DOComplete();
                _pizzaDisplay.transform.DOShakePosition(_shakeDuration, _shakeStrength);
            }
        }

        private void SpawnDamageParticles()
        {
            if (_applyDamageEffectPrefab == null) 
                return;

            ParticleSystem vfx = Instantiate(_applyDamageEffectPrefab, _effectSpawnPoint.position, Quaternion.identity);
            ParticleSystem.MainModule main = vfx.main;
            main.stopAction = _vfxStopAction;
        }
    }
}