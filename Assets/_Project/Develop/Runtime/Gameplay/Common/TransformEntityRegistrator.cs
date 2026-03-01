using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Common
{
    public class TransformEntityRegistrator : MonoEntityRegistrator
    {
        public Entity Instance { get; private set; }

        public override void Register(Entity entity)
        {
            Instance = entity;
            entity.AddTransform(GetComponent<Transform>());
        }
    }
}
