using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    // Запрос прыжка от Input
    public class JumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    // Сила прыжка (из конфига)
    public class JumpForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    // Находится ли на земле
    public class IsGrounded : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    // Кастомная гравитация (множитель)
    public class GravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    // Кол-во доступных прыжков (для double jump потом)
    public class JumpsAvailable : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class MaxJumps : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}