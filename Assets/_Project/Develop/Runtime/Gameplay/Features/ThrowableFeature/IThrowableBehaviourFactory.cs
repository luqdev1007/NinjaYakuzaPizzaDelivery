using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public interface IThrowableBehaviourFactory
    {
        ThrowableProjectile Create(ThrowableConfig config, Rigidbody2D rigidbody, Transform transform);
    }
}