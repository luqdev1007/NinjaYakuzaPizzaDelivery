using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "SlideConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Slide Config")]
    public class SlideConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float Duration { get; private set; } = 0.4f;
        [field: SerializeField, Min(0)] public float Speed { get; private set; } = 15f;
        [field: SerializeField, Min(0)] public float SlopeBoostMultiplier { get; private set; } = 2f;
        [field: SerializeField] public Vector2 SlopeJumpForce { get; private set; } = new Vector2(10f, 6f);
        [field: SerializeField] public LayerMask SlopeMask { get; private set; }
    }
}