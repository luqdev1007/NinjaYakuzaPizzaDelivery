using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Utilites.ObjectsManagment
{
    public class VfxPoolService : IVfxPoolService
    {
        private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> _pools = new();
        private readonly Transform _root;

        public VfxPoolService()
        {
            _root = new GameObject("VFX_Pool_Root").transform;
            Object.DontDestroyOnLoad(_root);
        }

        public ParticleSystem Spawn(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(prefab))
                _pools[prefab] = new Queue<ParticleSystem>();

            ParticleSystem instance;

            if (_pools[prefab].Count > 0)
            {
                instance = _pools[prefab].Dequeue();
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = Object.Instantiate(prefab, position, rotation, _root);
                if (!instance.TryGetComponent(out PoolableVfx returner))
                {
                    returner = instance.gameObject.AddComponent<PoolableVfx>();
                }
                returner.Setup(this, prefab);
            }

            instance.Play();
            return instance;
        }

        public void ReturnToPool(ParticleSystem prefab, ParticleSystem instance)
        {
            instance.gameObject.SetActive(false);
            _pools[prefab].Enqueue(instance);
        }
    }
}
