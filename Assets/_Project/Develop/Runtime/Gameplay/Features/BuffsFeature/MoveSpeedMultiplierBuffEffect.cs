using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class MoveSpeedMultiplierBuffEffect : IBuffEffect, IStatModifier
    {
        private readonly float _multiplier;

        public MoveSpeedMultiplierBuffEffect(float multiplier)
        {
            _multiplier = multiplier;
        }

        public void Apply(Entity entity)
        {
            entity.MoveSpeedModifiers.Add(this);
        }

        public void Remove(Entity entity)
        {
            entity.MoveSpeedModifiers.Remove(this);
        }

        public float Apply(float baseValue)
        {
            return baseValue * _multiplier;
        }
    }
}