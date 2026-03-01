using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Explosions
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Explosions/New Explosion Config", fileName = "ExplosionConfig")]
    public class ExplosionConfig : ScriptableObject
    {
        [field: SerializeField] public ExplosionType Type { get; private set; }
        [field: SerializeField] public ExplosionView ViewPrefab { get; private set; }
        [field: SerializeField] public float Range { get; private set; } = 5;
        [field: SerializeField] public float Power { get; private set; } = 1;
    }
}
