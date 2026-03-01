using System.Collections;
using UnityEngine;

public class Pistol : Weapon
{
    private Coroutine _internalReloadingCoroutine;
    private WaitForSeconds _reloadDelay;

    public Pistol(
        WeaponConfig config, 
        Transform weaponPoint, 
        MonoBehaviour coroutineRunner) : 
        base(config, weaponPoint, coroutineRunner)
    {
        _reloadDelay = new WaitForSeconds(1/Config.ShootsPerSecond);
    }

    public override bool TryShoot()
    {
        if (_internalReloadingCoroutine != null)
            return false;

        _internalReloadingCoroutine = CoroutineRunner.StartCoroutine(InternalReloadingProcess());

        Shoot();

        return true;
    }

    protected virtual void Shoot()
    {
        Projectile projectile = Object.Instantiate(Config.ProjectilePrefab, View.ShootPoint);
        SetupFor(projectile);
    }

    private void SetupFor(Projectile projectile)
    {
        projectile.transform.SetParent(null);
        projectile.Launch(Config.Damage, Config.ProjectileSpeed);
    }

    private IEnumerator InternalReloadingProcess()
    {
        yield return _reloadDelay;

        _internalReloadingCoroutine = null;
    }
}
