using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;
using UnityEngine;

public class SecretChestAnimatedView : EntityView
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openAnimName = "Open";

    private IDisposable _healthDisposable;

    protected override void OnEntityStartedWork(Entity entity)
    {
        _healthDisposable = entity.CurrentHealth.Subscribe((oldHp, newHp) =>
        {
            if (newHp <= 0)
            {
                OpenChest();
            }
        });
    }

    private void OpenChest()
    {
        _animator.SetTrigger(_openAnimName);
        // звук открытия через AudioService
    }

    public override void Cleanup(Entity entity)
    {
        base.Cleanup(entity);
        _healthDisposable?.Dispose();
    }
}