using System;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [Serializable]
    public class LifeCycleConfig
    {
        public float MaxHealth = 100f;
        public float SpawnProcessTime = 1f;
        public float DeathProcessTime = 2f;
    }
}