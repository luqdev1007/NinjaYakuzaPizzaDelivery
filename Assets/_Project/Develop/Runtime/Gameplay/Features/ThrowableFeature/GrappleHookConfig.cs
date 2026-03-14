using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    [CreateAssetMenu(fileName = "GrappleHookConfig", menuName = "Configs/Throwable/GrappleHook")]
    public class GrappleHookConfig : ThrowableConfig
    {
        [field: SerializeField] public float GrappleSpeed { get; private set; } = 15f;
        [field: SerializeField] public float ArriveDistance { get; private set; } = 0.5f;
        [field: SerializeField] public float ArrivalBounce { get; private set; } = 6f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
    }
}