using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InventoryFeature
{
    public class CurrentItemIndex : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class IsUsingItem : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class ItemUsedEvent : IEntityComponent
    {
        public ReactiveEvent<InventoryItemConfig> Value;
    }
}
