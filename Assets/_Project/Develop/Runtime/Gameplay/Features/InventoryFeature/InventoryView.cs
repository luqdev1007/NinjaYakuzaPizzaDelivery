using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

public class InventoryView : EntityView
{
    [SerializeField] private Animator _animator;
    private static readonly int ThrowTrigger = Animator.StringToHash("Throw");

    protected override void OnEntityStartedWork(Entity entity)
    {
        entity.ThrowEvent.Subscribe(OnThrow);
    }

    private void OnThrow() => _animator.SetTrigger(ThrowTrigger);
}