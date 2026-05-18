using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class CanSlide : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class SlideHitBoxSize : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class IsSliding : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class SlideCooldown : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlideDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlideSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}