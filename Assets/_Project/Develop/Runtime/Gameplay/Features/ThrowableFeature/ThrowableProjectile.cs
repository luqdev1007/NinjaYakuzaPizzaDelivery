using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public abstract class ThrowableProjectile
    {
        public event Action OnCompleted;
        protected readonly ICoroutinesPerformer CoroutinesPerformer;
        protected readonly ThrowableConfig Config;
        public GameObject Instance { get; protected set; }

        protected ThrowableProjectile(ThrowableConfig config, ICoroutinesPerformer coroutinesPerformer)
        {
            Config = config;
            CoroutinesPerformer = coroutinesPerformer;
        }

        public virtual void Launch(Vector3 from, Vector3 direction)
        {
            GameObject prefab = Resources.Load<GameObject>(Config.PrefabPath);
            if (prefab == null) return;

            Instance = Object.Instantiate(prefab, from, Quaternion.identity);
            CoroutinesPerformer.StartPerform(FlyCoroutine(direction));
        }

        public virtual void Cancel() => Destroy();

        protected virtual void Destroy()
        {
            OnCompleted?.Invoke();
            OnCompleted = null;
            if (Instance != null) { Object.Destroy(Instance); Instance = null; }
        }

        protected IEnumerator FlyCoroutine(Vector3 direction)
        {
            Vector3 startPosition = Instance.transform.position;
            Vector3 lastPosition = startPosition;

            while (Instance != null)
            {
                Instance.transform.position += direction * Config.ProjectileSpeed * Time.deltaTime;
                ApplyRotation(direction);

                if (Vector3.Distance(startPosition, Instance.transform.position) >= Config.MaxDistance)
                {
                    OnMaxDistanceReached(startPosition);
                    yield break;
                }

                RaycastHit2D hit = Physics2D.Linecast(lastPosition, Instance.transform.position, Config.HitMask);
                if (hit.collider != null)
                {
                    // Передаем именно точку попадания hit.point!
                    OnHitAtPoint(hit.point, hit.collider);
                    yield break;
                }

                lastPosition = Instance.transform.position;
                yield return null;
            }
        }

        protected virtual void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            AudioSource audioSource = Instance.GetComponentInChildren<AudioSource>();

            if (audioSource != null)
                audioSource.Stop();
        }

        protected virtual void ApplyRotation(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        protected virtual void OnMaxDistanceReached(Vector3 startPosition) => Destroy();
    }
}