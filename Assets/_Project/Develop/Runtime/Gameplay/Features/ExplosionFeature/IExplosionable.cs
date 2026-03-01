using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public interface IExplosionable
    {
        void Explode(float power, Vector3 sourcePosition, IExplosion source);
    }
}