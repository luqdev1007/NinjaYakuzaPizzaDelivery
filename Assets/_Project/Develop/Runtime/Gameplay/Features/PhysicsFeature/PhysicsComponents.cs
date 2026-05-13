using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class AngularDrag : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LinearDrag : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class Velocity : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class CanPhysicalyInteract : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsGrounded : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}


