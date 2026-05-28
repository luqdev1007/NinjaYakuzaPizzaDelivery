using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using System;
using UnityEngine;
using DG.Tweening;

public class PizzaHealthView : EntityView
{
    [Header("Pizza Visual")]
    [SerializeField] private PizzaDisplayView _pizzaDisplay;

    [Header("UI Elements")]
    [SerializeField] private LivesCountView _livesUI;

    [Header("Settings")]
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 0.5f;

    private IDisposable _damageEventDisposable;
    private Entity _linkedEntity;

    protected override void OnEntityStartedWork(Entity entity)
    {
        _linkedEntity = entity;
        _pizzaDisplay.Initialize(entity);

        _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);

        // Первичная синхронизация при старте БЕЗ тряски экрана и эффектов появления
        SyncVisuals(triggerEffects: false);
    }

    private void OnDamaged(DamageData data)
    {
        SyncVisuals(triggerEffects: true);
    }

    private void SyncVisuals(bool triggerEffects)
    {
        if (_pizzaDisplay == null || _linkedEntity == null) return;

        int currentSlices = _pizzaDisplay.UpdateHealthVisual(_linkedEntity.CurrentHealth.Value, _linkedEntity.MaxHealth.Value);

        if (triggerEffects)
        {
            // Трясем пиццу при уроне
            _pizzaDisplay.transform.DOComplete();
            _pizzaDisplay.transform.DOShakePosition(_shakeDuration, _shakeStrength);

            if (_livesUI != null)
            {
                _livesUI.Show(currentSlices);
            }
        }
        else
        {
            // На старте просто тихо обновляем текст
            if (_livesUI != null)
            {
                _livesUI.Show(currentSlices);
            }
        }
    }

    public override void Cleanup(Entity entity)
    {
        base.Cleanup(entity);
        _damageEventDisposable?.Dispose();
        if (_pizzaDisplay != null) _pizzaDisplay.transform.DOKill();
    }
}