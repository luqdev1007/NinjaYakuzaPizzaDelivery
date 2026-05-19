using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Main Hero/New Main Hero Config")]
    public class MaiHeroConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float MoveSpeedMin { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Deceleration { get; private set; }
        [field: SerializeField] public int MaxExtraJumps { get; private set; }
        [field: SerializeField] public float JumpForceBase { get; private set; }
        [field: SerializeField] public float JumpForceMax { get; private set; }
        [field: SerializeField] public float JumpChargeTime { get; private set; }
        [field: SerializeField] public float DashForceMin { get; private set; }
        [field: SerializeField] public float DashForceMax { get; private set; }
        [field: SerializeField] public float DashChargeTime { get; private set; }
        [field: SerializeField] public float DashCooldown { get; private set; }
        [field: SerializeField] public float DashDuration { get; private set; }
        [field: SerializeField] public float AirDashMultiplier { get; private set; }
        [field: SerializeField] public float AirDashVerticalBoost { get; private set; }
        [field: SerializeField] public float DashDamage { get; private set; }
        [field: SerializeField] public Vector2 DashHitboxSize { get; private set; }
        [field: SerializeField] public float SlideDuration { get; private set; }
        [field: SerializeField] public float SlideSpeed { get; private set; }
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        [field: SerializeField] public float AirJumpMultiplier {  get; private set; }
        [field: SerializeField] public float MinFallVelocity { get; private set; }
        [field: SerializeField] public float GlideMaxFallSpeed { get; private set; }
        [field: SerializeField] public float GlideSpeedDamping { get; private set; }
        [field: SerializeField] public float GlideBounceForce { get; private set; }
        [field: SerializeField] public float GlideSnapSpeed { get; private set; }
        [field: SerializeField] public float GlideSnapDuration { get; private set; }
        [field: SerializeField] public float GlideHorizontalDrag { get; private set; }
    }
}