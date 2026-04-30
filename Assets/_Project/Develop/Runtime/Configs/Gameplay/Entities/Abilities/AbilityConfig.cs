using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public string AbilityName { get; private set; }
        [field: SerializeField] public bool IsEnabledByDefault { get; private set; } = true;
    }
}