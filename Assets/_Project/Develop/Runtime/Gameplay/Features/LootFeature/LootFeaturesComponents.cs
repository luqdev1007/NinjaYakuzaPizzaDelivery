using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootPickedEvent : IEntityComponent
    {
        public ReactiveEvent<LootTypes> Value;
    }

    public class LootCollectSoundId : IEntityComponent
    {
        public ReactiveVariable<string> Value;
    }

    

    public class LootType : IEntityComponent
    {
        public ReactiveVariable<LootTypes> Value;
    }

    public class LootCount : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class LootIsDropped : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class LootIsCollected : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class LootCollectRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LootInitialLifeTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LootCurrentLifeTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class CanDropLoot : IEntityComponent
    {
        public ICompositeCondition Value;
    }
}
