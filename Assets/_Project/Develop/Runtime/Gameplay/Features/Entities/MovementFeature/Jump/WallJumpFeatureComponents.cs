using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
{
    public class CanWallJump : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class WallJumpForceMultiplier : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    public class WallMask : IEntityComponent
    {
        public LayerMask Value;
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

