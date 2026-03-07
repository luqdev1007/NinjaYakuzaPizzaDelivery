using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public abstract class ThrowableProjectile
    {
        protected readonly ICoroutinesPerformer CoroutinesPerformer;
        protected readonly ThrowableConfig Config;
        protected GameObject Instance;

        protected ThrowableProjectile(ThrowableConfig config, ICoroutinesPerformer coroutinesPerformer)
        {
            Config = config;
            CoroutinesPerformer = coroutinesPerformer;
        }

        public void Launch(Vector3 from, Vector3 direction)
        {
            GameObject prefab = Resources.Load<GameObject>(Config.PrefabPath);

            if (prefab == null)
            {
                Debug.LogError($"ThrowableProjectile: префаб не найден по пути '{Config.PrefabPath}'");
                return;
            }

            Instance = Object.Instantiate(prefab, from, Quaternion.identity);
            CoroutinesPerformer.StartPerform(FlyCoroutine(direction));
        }

        public void Cancel()
        {
            Destroy();
        }

        protected virtual void Destroy()
        {
            if (Instance != null)
            {
                Object.Destroy(Instance);
                Instance = null;
            }
        }

        protected IEnumerator FlyCoroutine(Vector3 direction)
        {
            Vector3 startPosition = Instance.transform.position;

            while (Instance != null)
            {
                Instance.transform.position += direction * Config.ProjectileSpeed * Time.deltaTime;

                float travelled = Vector3.Distance(startPosition, Instance.transform.position);

                if (travelled >= Config.MaxDistance)
                {
                    OnMaxDistanceReached(startPosition);
                    yield break;
                }

                Collider2D hit = Physics2D.OverlapPoint(Instance.transform.position, Config.HitMask);

                if (hit != null)
                {
                    OnHit(hit);
                    yield break;
                }

                yield return null;
            }
        }

        protected virtual void OnHit(Collider2D hit) => Destroy();
        protected virtual void OnMaxDistanceReached(Vector3 startPosition) => Destroy();
    }
}