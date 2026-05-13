using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleHookProjectile : ThrowableProjectile
    {
        public GrappleHookProjectile(ThrowableConfig config, ICoroutinesPerformer coroutinesPerformer) : base(config, coroutinesPerformer)
        {
        }

        public event Action<Vector2, Collider2D> OnAnchored;


        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            base.OnHitAtPoint(point, hit);

            OnAnchored?.Invoke(point, hit);
            Destroy();
        }
    }
}