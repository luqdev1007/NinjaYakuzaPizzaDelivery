using UnityEngine;

public class Shotgun : Pistol
{
    private readonly float[] _angles = { -15f, 0f, 15f };

    public Shotgun(WeaponConfig config, Transform weaponPoint, MonoBehaviour coroutineRunner) : base(config, weaponPoint, coroutineRunner)
    {
        
    }

    protected override void Shoot()
    {
        Vector3 baseDirection = View.ShootPoint.right;

        foreach (var angle in _angles)
        {
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle + Random.Range(-5f, 5f));
            Vector3 finalDirection = spreadRotation * baseDirection;
            Projectile projectile = Object.Instantiate(Config.ProjectilePrefab, View.ShootPoint);
            SetupFor(projectile, finalDirection);
        }
    }

    private void SetupFor(Projectile projectile, Vector3 direction)
    {
        projectile.transform.right = direction.normalized;
        projectile.transform.SetParent(null);
        projectile.Launch(Config.Damage, Config.ProjectileSpeed);
    }
}