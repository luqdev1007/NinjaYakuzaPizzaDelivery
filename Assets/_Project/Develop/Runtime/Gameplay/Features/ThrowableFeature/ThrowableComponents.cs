using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class CurrentThrowableIndex : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class GrappleCharges : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class ShurikenCharges : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class SleepDartCharges : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class IsThrowing : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
