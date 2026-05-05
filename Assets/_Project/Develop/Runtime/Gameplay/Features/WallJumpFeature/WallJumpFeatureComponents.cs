using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class CanWallJump : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class WallJumpLockTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class IsWallJumping : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class WallJumpParams : IEntityComponent
    {
        public float MinVelocityY;
        public Vector2 JumpForce;
        public float ControlLockDuration;
    }

    public class WallNormal : IEntityComponent { public ReactiveVariable<Vector2> Value = new(Vector2.zero); }
}

