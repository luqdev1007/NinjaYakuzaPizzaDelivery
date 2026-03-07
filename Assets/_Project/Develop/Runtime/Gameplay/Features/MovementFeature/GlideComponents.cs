using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class CanGlide : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsGliding : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class GlideMaxFallSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GlideSpeedDamping : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GlideBounceForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}