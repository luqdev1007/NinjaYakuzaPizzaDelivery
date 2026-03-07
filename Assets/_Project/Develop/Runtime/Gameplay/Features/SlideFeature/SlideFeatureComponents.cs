using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{

    public class CanSlide : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class CanPlunge : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsSliding : IEntityComponent { public ReactiveVariable<bool> Value = new(); }
    public class IsPlunging : IEntityComponent { public ReactiveVariable<bool> Value = new(); }
    public class SlideDuration : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlideSpeed : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeBoostMultiplier : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeJumpForce : IEntityComponent { public ReactiveVariable<Vector2> Value; }
    public class PlungeSpeed : IEntityComponent { public ReactiveVariable<float> Value; }
    public class PlungeAOERadius : IEntityComponent { public ReactiveVariable<float> Value; }
    public class PlungeAOEDamage : IEntityComponent { public ReactiveVariable<float> Value; }
    public class PlungeKnockbackForce : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeMask : IEntityComponent { public LayerMask Value; }
}
