using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class IsSecretChest : IEntityComponent { }

    public class LootPickedEvent : IEntityComponent
    {
        public ReactiveEvent<LootType> Value;
    }

    public class CollectRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AutoDeleteInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AutoDeleteCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsPullable : IEntityComponent
    {
    }

    public class IsPullingProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IsCollected : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }


    public class Coins : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class LootIsDropped : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class CanDropLoot : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class LootTypeNew : IEntityComponent
    {
        public ReactiveVariable<LootType> Value;
    }

    // ??
    public class LootTag : IEntityComponent { }
    public class ExperienceValue : IEntityComponent { public ReactiveVariable<float> Value; }
    public class CollectableInProcess : IEntityComponent { public ReactiveVariable<bool> Value; }
}
