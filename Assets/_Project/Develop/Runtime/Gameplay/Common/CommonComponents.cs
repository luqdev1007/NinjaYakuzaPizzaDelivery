using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Common
{
    public class RigidbodyComponent : IEntityComponent
    {
        public Rigidbody2D Value;
    }

    public class TransformComponent : IEntityComponent
    {
        public Transform Value;
    }

    public class IsAsleep : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }


}