using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce
{
    /// <summary>
    /// Просьба на отскок от внешнего источника (трамплин и т.п.).
    /// Ось передаётся явно: семантика отскока — «заменить компоненту скорости вдоль
    /// UpAxis на LaunchVelocity, тангенциальную сохранить». Голым вектором это не
    /// выражается, поэтому ось и величина едут раздельно.
    /// </summary>
    public struct BounceImpulseData
    {
        public Vector2 UpAxis;
        public float LaunchVelocity;
    }

    /// <summary>
    /// Входящая просьба извне по конвенции *Request (образец — TakeDamageRequest):
    /// внешний актор зовёт Invoke, применяет — система сущности, владеющая её
    /// rigidbody. Прямой записи в чужой rigidbody мимо арбитража быть не должно.
    /// </summary>
    public class BounceImpulseRequest : IEntityComponent
    {
        public ReactiveEvent<BounceImpulseData> Value;
    }
}
