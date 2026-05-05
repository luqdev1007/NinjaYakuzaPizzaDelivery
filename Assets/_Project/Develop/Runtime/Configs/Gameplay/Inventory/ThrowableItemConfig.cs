using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Throwable")]
    public class ThrowableItemConfig : ConsumableConfig
    {
        public ThrowableConfig ProjectileSettings;

        public override void Use(Entity user, IThrowableBehaviourFactory factory)
        {
            Transform transform = user.Transform;

            Vector2 mousePos = user.MouseWorldPositionInput.Value;

            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

            var projectile = factory.Create(ProjectileSettings, user.Rigidbody, transform);
            projectile.Launch(transform.position, direction);

            Debug.Log($"Брошен предмет: {Name}");
        }
    }
}