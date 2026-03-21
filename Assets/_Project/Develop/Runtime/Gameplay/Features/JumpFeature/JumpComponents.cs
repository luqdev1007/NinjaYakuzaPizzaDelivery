using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpForceMax : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpChargeTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class JumpForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsGrounded : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class GravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpsAvailable : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class MaxJumps : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}