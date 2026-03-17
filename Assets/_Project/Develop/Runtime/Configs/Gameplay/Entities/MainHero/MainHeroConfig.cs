using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Main Hero Config")]
    public class MainHeroConfig : EntityConfig
    {
        [Header("Common Settings")]
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float MinFallVelocityForAction { get; private set; } = -2f;

        [Header("Throwables")]
        [field: SerializeField] public GrappleHookConfig GrappleConfig { get; private set; }
        [field: SerializeField] public ShurikenConfig ShurikenConfig { get; private set; }
        [field: SerializeField] public SleepDartConfig SleepDartConfig { get; private set; }

        [Header("Configs")]
        [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
        [field: SerializeField] public JumpConfig JumpConfig { get; private set; }
        [field: SerializeField] public GlideConfig GlideConfig { get; private set; }
        [field: SerializeField] public DashConfig DashConfig { get; private set; }
        [field: SerializeField] public AttackConfig AttackConfig { get; private set; }
        [field: SerializeField] public LifeCycleConfig LifeCycleConfig { get; private set; }
        [field: SerializeField] public WallHangConfig WallHangConfig { get; private set; }
        [field: SerializeField] public SlideConfig SlideConfig { get; private set; }
        [field: SerializeField] public PlungeConfig PlungeConfig { get; private set; }
    }
}