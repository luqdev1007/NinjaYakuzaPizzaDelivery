using UnityEngine;

public interface IVfxPoolService
    {
        ParticleSystem Spawn(ParticleSystem prefab, Vector3 position, Quaternion rotation);
        void ReturnToPool(ParticleSystem prefab, ParticleSystem instance);
    }
