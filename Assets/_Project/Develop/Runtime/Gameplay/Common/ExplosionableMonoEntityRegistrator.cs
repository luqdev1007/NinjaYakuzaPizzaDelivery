using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Common
{
    public class ExplosionableMonoEntityRegistrator : MonoEntityRegistrator
    {
        [SerializeField] private ExplosionableMono _explosionableMono;

        public override void Register(Entity entity)
        {
            entity.AddExplosionableMono(_explosionableMono);
        }
    }
}
