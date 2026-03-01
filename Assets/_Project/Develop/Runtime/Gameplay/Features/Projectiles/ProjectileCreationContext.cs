using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class ProjectileCreationContext
    {
        public Entity Owner { get; private set; }
        public float LaunchPower { get; private set; }
        public float FinalDamage { get; private set; }
        public float LaunchDelay { get; private set; }
        public Vector3 ShootDirection { get; private set; }

        public ProjectileCreationContext(
            Entity owner,
            float launchPower,
            float finalDamage,
            float launchDelay,
            Vector3 shootDirection)
        {
            Owner = owner;
            LaunchPower = launchPower;
            FinalDamage = finalDamage;
            LaunchDelay = launchDelay;
            ShootDirection = shootDirection;
        }
    }
}
