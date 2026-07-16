using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    // Время, ПРОШЕДШЕЕ с открытия knockback-окна. Растёт от 0 до KnockbackDuration.
    // Пока elapsed < duration — окно активно и движение сущности приостановлено.
    // Раньше называлось KnockbackInitialTimer, что читалось как «начальное значение»
    // и было источником инверсии условия canMove.
    public class KnockbackElapsedTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }


    // ДЛИТЕЛЬНОСТЬ knockback-окна, константа из конфига. Раньше называлось
    // KnockbackTimer, хотя ничего не «тикало» — это верхняя граница для elapsed.
    public class KnockbackDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class BaseGravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LinearDrag : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class Velocity : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class CanPhysicalyInteract : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsGrounded : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
