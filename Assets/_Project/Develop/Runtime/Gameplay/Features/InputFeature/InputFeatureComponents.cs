using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class InputState
    {
        public ReactiveVariable<bool> IsPressed = new(false);
        public ReactiveVariable<bool> IsHeld = new(false);
        public ReactiveVariable<bool> IsReleased = new(false);
    }

    public class AttackInput : IEntityComponent
    {
        public InputState Value;
    }

    public class GrappleInput : IEntityComponent
    {
        public InputState Value;
    }

    public class MoveDirectionInput : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class InventoryScrollDelta : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpInput : IEntityComponent
    {
        public InputState Value;
    }

    public class GlideActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class DashInput : IEntityComponent
    {
        public InputState Value;
    }

    public class SlideRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class PlungeActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class ThrowProjectileRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class GrapplingHookActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class UltimateRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class MouseWorldPositionInput : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new(Vector2.zero);
    }

    public class ThrowInput : IEntityComponent
    {
        public InputState Value = new();
    }

    public class ShowTargetActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class AutoTargetToggleRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CycleTargetRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }
}