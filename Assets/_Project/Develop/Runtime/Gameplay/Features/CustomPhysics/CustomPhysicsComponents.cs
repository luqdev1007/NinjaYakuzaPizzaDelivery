using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics
{
    public class GravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PushForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PushDirection : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }
}
