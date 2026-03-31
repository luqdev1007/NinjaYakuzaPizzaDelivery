using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement; // Добавлено
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleHookProjectile : ThrowableProjectile
    {
        public event Action<Vector2, Collider2D> OnAnchored;
        private readonly AudioService _audioService; // Добавлено

        // Обновленный конструктор
        public GrappleHookProjectile(GrappleHookConfig config, ICoroutinesPerformer coroutinesPerformer, AudioService audioService)
            : base(config, coroutinesPerformer)
        {
            _audioService = audioService;
        }

        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            base.OnHitAtPoint(point, hit);

            // Определяем звук попадания
            string hitSfx = hit.CompareTag("Enemy") ? "EnemyHitHook" : "WallHitHook";
            _audioService.PlaySfxByPrefixAuto(hitSfx, UnityEngine.Random.Range(0.95f, 1.05f));

            OnAnchored?.Invoke(point, hit);
            Destroy();
        }
    }
}