using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class CanPlunge : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsPlunging : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class PlungeSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactRangeMin : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactDamageMin: IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactKnockbackForceMin : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}