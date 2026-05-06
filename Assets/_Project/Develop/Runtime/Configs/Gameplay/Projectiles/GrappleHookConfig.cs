using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "New Grapple Hook Config", menuName = "Configs/Gameplay/Projectiles/GrappleHook")]
    public class GrappleHookConfig : ThrowableConfig
    {
        [field: SerializeField] public float GrappleSpeed { get; private set; } = 15f;
        [field: SerializeField] public float ArriveDistance { get; private set; } = 0.5f;
    }
}