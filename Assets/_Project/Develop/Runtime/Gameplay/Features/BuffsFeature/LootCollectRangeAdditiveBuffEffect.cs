using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class LootCollectRangeAdditiveBuffEffect : IBuffEffect, IStatModifier
    {
        private readonly float _additiveAmount;

        public LootCollectRangeAdditiveBuffEffect(float additiveAmount)
        {
            _additiveAmount = additiveAmount;
        }

        public void Apply(Entity entity)
        {
            entity.LootCollectRangeModifiers.Add(this);
        }

        public void Remove(Entity entity)
        {
            entity.LootCollectRangeModifiers.Remove(this);
        }

        public float Apply(float baseValue)
        {
            return baseValue + _additiveAmount;
        }
    }
}