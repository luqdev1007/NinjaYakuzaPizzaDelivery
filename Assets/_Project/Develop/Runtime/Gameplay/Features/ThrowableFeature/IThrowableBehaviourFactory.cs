using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public interface IThrowableBehaviourFactory
    {
        ThrowableProjectile Create(ThrowableItemConfig config);
        object Create(ThrowableItemConfig throwableConfig, Rigidbody2D rigidbody, Transform transform);
    }
}