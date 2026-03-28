using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "GrappleHookConfig", menuName = "Configs/Gameplay/Projectiles/GrappleHook")]
    public class GrappleHookConfig : ThrowableConfig
    {
        [field: SerializeField] public float GrappleSpeed { get; private set; } = 15f;
        [field: SerializeField] public float ArriveDistance { get; private set; } = 0.5f;
        [field: SerializeField] public float ArrivalBounce { get; private set; } = 6f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public float CancelInertiaMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float ArrivalMinXComponent { get; private set; } = 0.3f;
        [field: SerializeField] public float InitialPopUpForce { get; private set; } = 4f;
    }
}