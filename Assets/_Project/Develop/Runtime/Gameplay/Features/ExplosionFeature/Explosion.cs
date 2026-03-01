using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{


    public class Explosion : IExplosion
    {
        private float _range;
        private float _power;

        public Explosion(float range, float power)
        {
            _range = range;
            _power = power;
        }

        public void Activate(Vector3 at, float extraRange = 0)
        {
            Collider[] targets = Physics.OverlapSphere(at, _range + extraRange);

            foreach (Collider target in targets)
            {
                IExplosionable actor = target.GetComponent<IExplosionable>();

                if (actor != null)
                {
                    actor.Explode(_power, at, this);
                }

                MonoEntity monoEntity = target.GetComponent<MonoEntity>();

                if (monoEntity != null)
                {
                    if (monoEntity.LinkedEntity.HasComponent<TakeDamageRequest>())
                    {
                        monoEntity.LinkedEntity.TakeDamageRequest.Invoke(1);
                        // Debug.Log(monoEntity.gameObject.name + " health: " + monoEntity.LinkedEntity.CurrentHealth.Value);
                    }
                }
            }

        }
    }
}