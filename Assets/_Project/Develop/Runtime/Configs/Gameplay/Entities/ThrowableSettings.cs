using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [Serializable]
    public class ThrowableSettings
    {
        [field: SerializeField] public GrappleHookConfig GrappleConfig { get; private set; }
        [field: SerializeField] public ShurikenConfig ShurikenConfig { get; private set; }
        [field: SerializeField] public SleepDartConfig SleepDartConfig { get; private set; }
    }
}