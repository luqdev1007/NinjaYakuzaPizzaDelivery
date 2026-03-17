using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "DashConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Dash Config")]
    public class DashConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float Duration { get; private set; } = 0.3f;
        [field: SerializeField, Min(0)] public float ForceMin { get; private set; } = 8f;
        [field: SerializeField, Min(0)] public float ForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float ChargeTime { get; private set; } = 0.4f;
        [field: SerializeField, Min(0)] public float Cooldown { get; private set; } = 0.5f;
    }
}