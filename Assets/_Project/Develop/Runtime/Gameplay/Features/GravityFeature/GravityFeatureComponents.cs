using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature
{
    public class BaseGravity : IEntityComponent
    {
        public ReactiveVariable<float> Value; // Стандарт (напр. 1.0)
    }

    public class GravityModifier : IEntityComponent
    {
        // Сюда системы (крюк, зоны) пишут множитель. 
        // По умолчанию 1.0. Если крюк — пишет 0.0.
        public ReactiveVariable<float> Value;
    }

    public class GravityDirection : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value; // По умолчанию (0, -1)
    }
}