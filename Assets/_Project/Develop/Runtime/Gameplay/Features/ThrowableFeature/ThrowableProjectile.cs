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

        public void Launch(Vector3 from, Vector3 direction)
        {
            GameObject prefab = Resources.Load<GameObject>(Config.PrefabPath);
            if (prefab == null) return;

            Instance = Object.Instantiate(prefab, from, Quaternion.identity);
            CoroutinesPerformer.StartPerform(FlyCoroutine(direction));
        }

        protected virtual void Destroy()
        {
            OnCompleted?.Invoke();
            OnCompleted = null;
            if (Instance != null) { Object.Destroy(Instance); Instance = null; }
        }

        protected IEnumerator FlyCoroutine(Vector3 direction)
        {
            Vector3 startPosition = Instance.transform.position;
            while (Instance != null)
            {
                Instance.transform.position += direction * Config.ProjectileSpeed * Time.deltaTime;
                ApplyRotation(direction);

                if (Vector3.Distance(startPosition, Instance.transform.position) >= Config.MaxDistance)
                {
                    OnMaxDistanceReached(startPosition);
                    yield break;
                }

                Collider2D hit = Physics2D.OverlapPoint(Instance.transform.position, Config.HitMask);
                if (hit != null)
                {
                    OnHit(hit);
                    yield break; // ВОТ ЭТО ВЕРНУЛИ. Попал = лететь перестал.
                }
                yield return null;
            }
        }

        public void Cancel()
        {
            Destroy();
        }

        protected virtual void ApplyRotation(Vector3 direction) { /* дефолтный поворот */ }
        protected virtual void OnHit(Collider2D hit) => Destroy();
        protected virtual void OnMaxDistanceReached(Vector3 startPosition) => Destroy();
    }
}