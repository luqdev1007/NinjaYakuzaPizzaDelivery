using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{


    public class CanWallHang : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsWallHanging : IEntityComponent { public ReactiveVariable<bool> Value = new(); }
    public class WallHangSlideSpeed : IEntityComponent { public ReactiveVariable<float> Value; }
    public class WallHangLayer : IEntityComponent { public LayerMask Value; }
    public class WallJumpForce : IEntityComponent { public ReactiveVariable<Vector2> Value; }
    public class WallDirection : IEntityComponent { public ReactiveVariable<float> Value = new(); }
}
