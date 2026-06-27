using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class BaseMoveSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MoveSpeedModifiers : IEntityComponent
    {
        public StatModifiersList Value;
    }

    public class BaseLootCollectRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LootCollectRangeModifiers : IEntityComponent
    {
        public StatModifiersList Value;
    }
}