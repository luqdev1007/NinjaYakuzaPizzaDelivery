using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Patrol
{
    // Конвенция EntityAPIGenerator: РОВНО ОДНО public-поле с именем Value.
    // Нарушишь — генератор выдаст урезанный API (без свойства-геттера и без
    // TryGet), и это не будет ошибкой компиляции, просто молча пропадут методы.

    /// <summary>
    /// Первый конец маршрута в МИРОВЫХ координатах. Снимается один раз при
    /// сборке сущности и дальше не меняется, поэтому не реактивен.
    /// </summary>
    public class PatrolPointA : IEntityComponent
    {
        public Vector2 Value;
    }

    /// <summary>
    /// Второй конец маршрута в МИРОВЫХ координатах. См. <see cref="PatrolPointA"/>.
    /// </summary>
    public class PatrolPointB : IEntityComponent
    {
        public Vector2 Value;
    }

    /// <summary>
    /// Текущая цель движения. ЕДИНСТВЕННЫЙ реактивный компонент патруля:
    /// его пишет состояние мозга, а читает система движения — то есть это
    /// точка стыка двух каналов (мозг и системы тикаются в одном FixedUpdate,
    /// но порядок между ними задаётся GameplayBootstrap, а не этим полем).
    ///
    /// Single-writer: писатель ровно один — PatrolState.
    /// </summary>
    public class MoveTargetPoint : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    /// <summary>
    /// Нижняя граница паузы на конце маршрута, сек.
    /// </summary>
    public class PatrolPauseMin : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Верхняя граница паузы на конце маршрута, сек.
    /// </summary>
    public class PatrolPauseMax : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Радиус достижения точки. Читают двое и по-разному, это не дублирование:
    /// состояние мозга — чтобы решить «пора вставать в паузу», система движения —
    /// чтобы обнулить скорость и не дёргаться вокруг цели.
    /// </summary>
    public class PatrolArriveDistance : IEntityComponent
    {
        public float Value;
    }
}
