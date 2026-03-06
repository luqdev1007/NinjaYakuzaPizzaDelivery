using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
    public class BodyCollider : IEntityComponent
    {
        public Collider2D Value;
    }

    public class GroundMask : IEntityComponent
    {
        public LayerMask Value;
    }

    public class ContactsDetectingMask : IEntityComponent
    {
        public LayerMask Value;
    }

    public class ContactCollidersBuffer : IEntityComponent
    {
        public Buffer<Collider2D> Value;
    }

    public class ContactEntitiesBuffer : IEntityComponent
    {
        public Buffer<Entity> Value;
    }

    public class DeathMask : IEntityComponent
    {
        public LayerMask Value;
    }

    public class IsTouchDeathMask : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IsTouchAnotherTeam : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
