using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public interface IBuffEffect
    {
        void Apply(Entity entity);
        void Remove(Entity entity);
    }
}