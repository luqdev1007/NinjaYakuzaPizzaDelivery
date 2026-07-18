using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Explosion
{
    /// <summary>
    /// Просьба на импульс от взрыва извне, по конвенции *Request (образцы —
    /// TakeDamageRequest, BounceImpulseRequest): внешний актор зовёт Invoke,
    /// применяет — система сущности, владеющая её rigidbody.
    /// </summary>
    /// <remarks>
    /// В отличие от BounceImpulseData ось отдельно не передаётся: у взрыва нет
    /// «своей» оси, вдоль которой надо заменить скорость. Направление и величина
    /// уже посчитаны источником (вектор от эпицентра к цели), поэтому едет один
    /// готовый вектор силы.
    /// </remarks>
    public class ExplosionImpulseRequest : IEntityComponent
    {
        public ReactiveEvent<Vector2> Value;
    }
}
