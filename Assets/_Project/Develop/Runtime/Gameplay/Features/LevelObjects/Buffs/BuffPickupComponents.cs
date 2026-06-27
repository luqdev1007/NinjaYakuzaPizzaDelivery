using Assets._Project.Develop.Runtime.Configs.Gameplay.Buffs;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffIsCollected : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class BuffPickupConfig : IEntityComponent
    {
        public BuffConfig Value;
    }
}