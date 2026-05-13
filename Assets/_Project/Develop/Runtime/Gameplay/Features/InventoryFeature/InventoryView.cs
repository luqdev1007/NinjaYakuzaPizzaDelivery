using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventoryView : EntityView
    {
        private static readonly int ThrowTrigger = Animator.StringToHash("Throw");

        [SerializeField] private Animator _animator;

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Подписка на бросок
            // entity.ThrowEvent.Subscribe(OnThrow);

            // Подписка на смену предмета
            // entity.CurrentThrowableIndex.Subscribe((oldIdx, newIdx) => OnItemSwitched());
        }

        private void OnThrow()
        {
            if (_animator) 
                _animator.SetTrigger(ThrowTrigger);
        }
    }
}