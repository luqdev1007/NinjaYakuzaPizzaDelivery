using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class IntentUseItem : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentSwitchItemDelta : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IntentAimDirection : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class IntentMovement : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class IntentGrapple : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentJump : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentSlide : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentDash : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentAttack : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentSwitchTarget : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
