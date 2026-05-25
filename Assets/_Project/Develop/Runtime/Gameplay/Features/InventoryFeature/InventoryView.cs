using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Inventory.Potions;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventoryView : EntityView
    {
        private static readonly int ThrowTrigger = Animator.StringToHash("Throw");
        private static readonly int DrinkTrigger = Animator.StringToHash("Drink");

        [SerializeField] private Animator _animator;

        protected override void OnEntityStartedWork(Entity entity)
        {
            entity.ItemUsedEvent.Subscribe(OnItemUsed);
            entity.CurrentItemIndex.Subscribe(OnItemSwitched);
        }

        private void OnItemUsed(InventoryItemConfig config)
        {
            if (_animator == null) 
                return;

            if (config is ThrowableItemConfig)
            {
                _animator.SetTrigger(ThrowTrigger);
            }
            else if (config is PotionItemConfig)
            {
                // _animator.SetTrigger(DrinkTrigger);
            }
        }

        private void OnItemSwitched(int oldIdx, int newIdx)
        {
            // Здесь будет логика для локального отображения предмета на спрайте персонажа, 
            // когда вы переключаете слоты (например, отобразить сюрикен в руке)
        }
    }
}