using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleHookProjectile : ThrowableProjectile
    {
        // Передаем позицию и коллайдер
        public event Action<Vector2, Collider2D> OnAnchored;

        public GrappleHookProjectile(GrappleHookConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer) { }

        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            OnAnchored?.Invoke(point, hit);
            // Снаряд уничтожится, но система уже знает локальное смещение
            Destroy();
        }
    }
}