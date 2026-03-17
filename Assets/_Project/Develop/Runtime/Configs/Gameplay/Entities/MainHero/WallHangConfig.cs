using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "WallHangConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Wall Hang Config")]
    public class WallHangConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask WallLayer { get; private set; }
        [field: SerializeField] public float SlideSpeed { get; private set; } = 1f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(8f, 12f);
    }
}
