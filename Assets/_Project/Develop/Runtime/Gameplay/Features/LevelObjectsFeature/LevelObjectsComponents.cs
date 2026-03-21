using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjectsFeature
{
    /*
    public class StartAttackRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }
    */

    public class CanApplyPhysicsFroce : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class ApplyingForceCharges : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class ApplyingForcePower : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
