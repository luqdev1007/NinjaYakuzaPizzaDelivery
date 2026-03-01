using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Weapon : IDisposable
{
    protected WeaponConfig Config;
    protected WeaponView View;
    protected MonoBehaviour CoroutineRunner;

    public Weapon(WeaponConfig config, Transform weaponPoint, MonoBehaviour coroutineRunner)
    {
        Config = config;
        View = Object.Instantiate(config.ViewPrefab, weaponPoint);
        CoroutineRunner = coroutineRunner;
    }

    public void Dispose()
    {
        Object.Destroy(View.gameObject);
    }

    public abstract bool TryShoot();
}
