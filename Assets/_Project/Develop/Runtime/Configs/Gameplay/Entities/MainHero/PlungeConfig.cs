using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "PlungeConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Plunge Config")]
    public class PlungeConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float Speed { get; private set; } = 25f;
        [field: SerializeField, Min(0)] public float AOERadius { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float AOEDamage { get; private set; } = 50f;
        [field: SerializeField, Min(0)] public float KnockbackForce { get; private set; } = 10f;
    }
}
