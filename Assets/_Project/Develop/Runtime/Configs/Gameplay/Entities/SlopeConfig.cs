using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [System.Serializable]
    public class SlopeConfig
    {
        [field: SerializeField] public float DownhillBaseForce { get; private set; } = 20f;
        [field: SerializeField] public float BoostMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float MagnetForce { get; private set; } = 5f;
        [field: SerializeField] public float MaxAccumSpeed { get; private set; } = 30f;
        [field: SerializeField] public float AccumGainRate { get; private set; } = 10f;
        [field: SerializeField] public float AccumDecayRate { get; private set; } = 5f;
        [field: SerializeField] public float SlideOffDelay { get; private set; } = 0.2f;
        [field: SerializeField] public float MinEjectVelocity { get; private set; } = 5f;
        [field: SerializeField] public float EjectForceMultiplier { get; private set; } = 1.2f;
        [field: SerializeField] public float AutoSlidePush { get; private set; } = 2f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(5f, 10f);
    }
}