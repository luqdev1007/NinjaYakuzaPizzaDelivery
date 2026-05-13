using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class CanWallJump : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class IsWallJumping : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class WallJumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class WallJumpRequest : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }
}

