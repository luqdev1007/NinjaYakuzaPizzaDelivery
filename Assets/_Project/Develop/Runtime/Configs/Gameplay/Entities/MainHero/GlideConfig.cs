using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "GlideConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Glide Config")]
    public class GlideConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxFallSpeed { get; private set; } = -2f;
        [field: SerializeField] public float SpeedDamping { get; private set; } = 5f;
        [field: SerializeField] public float BounceForce { get; private set; } = 4f;
    }
}