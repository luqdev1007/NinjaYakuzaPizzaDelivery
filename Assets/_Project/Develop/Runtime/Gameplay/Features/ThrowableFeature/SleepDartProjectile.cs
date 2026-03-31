using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class SleepDartProjectile : ThrowableProjectile
    {
        public SleepDartProjectile(ThrowableConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer) { }

        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            base.OnHitAtPoint(point, hit);

            var monoEntity = hit.GetComponentInParent<MonoEntity>();
            if (monoEntity != null)
            {
                // Тут будет твоя логика усыпления (например, добавление компонента IsAsleep)
                Debug.Log("Враг усыплен!");
            }

            Destroy(); // Дротики обычно не втыкаются, а ломаются/тратятся
        }

        protected override void ApplyRotation(Vector3 direction)
        {
            // Дротик летит носом вперед, а не крутится
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}