namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    using UnityEngine;

    public class ExplosionView : MonoBehaviour
    {
        public Explosion ExplosionEffect { get; private set; }

        public void Initialize(Explosion effect, bool activateOnCreate = false, float selfDestroyTime = 2)
        {
            ExplosionEffect = effect;

            if (activateOnCreate)
                effect.Activate(transform.position);

            Destroy(gameObject, selfDestroyTime);
        }
    }
}