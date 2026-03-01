using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Common
{
    public class RigidbodyComponent : IEntityComponent
    {
        public Rigidbody Value;
    }

    public class TransformComponent : IEntityComponent
    {
        public Transform Value;
    }

    public class CharacterControllerComponent : IEntityComponent
    {
        public CharacterController Value;
    }

    public class AnimatorComponent : IEntityComponent
    {
        public Animator Value;
    }

    public class ExplosionableComponent : IEntityComponent
    {
        public IExplosionable Value;
    }

    public class ExplosionableMonoComponent : IEntityComponent
    {
        public ExplosionableMono Value;
    }
}
