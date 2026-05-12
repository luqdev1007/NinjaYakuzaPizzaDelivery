using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootPickedEvent : IEntityComponent
    {
        public ReactiveEvent<LootType> Value;
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
