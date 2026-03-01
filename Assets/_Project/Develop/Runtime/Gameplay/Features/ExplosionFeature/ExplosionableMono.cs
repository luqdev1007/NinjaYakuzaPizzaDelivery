using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public abstract class ExplosionableMono : MonoBehaviour, IExplosionable
    {
        [field: SerializeField] public Rigidbody AttachedRigidbody { get; private set; }

        public abstract void Explode(float power, Vector3 sourcePosition, IExplosion source);
    }
}