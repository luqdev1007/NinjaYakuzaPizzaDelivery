using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class ExplosionRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ExplosionDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MustExplode : IEntityComponent
    {
        public ICompositeCondition Value;
    }
}
