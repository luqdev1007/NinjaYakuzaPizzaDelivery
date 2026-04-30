using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Main Hero/New Modular Hero Config")]
    public class MainHeroConfig : EntityConfig
    {
        [Header("Common & Physics")]
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float LootCollectRange { get; private set; } = 3f;
        [field: SerializeField] public float MinFallVelocityForAction { get; private set; } = -2f;
        [field: SerializeField] public LayerMask GroundMask { get; private set; }

        [Header("Ability Modules")]
        [SerializeField] private MovementAbilityConfig _movement;
        [SerializeField] private JumpAbilityConfig _jump;
        [SerializeField] private DashAbilityConfig _dash;
        [SerializeField] private GlideAbilityConfig _glide;
        [SerializeField] private WallAbilityConfig _wall;
        [SerializeField] private CombatAbilityConfig _combat;

        [Header("Physics Modules")]
        [SerializeField] private SlopeConfig _slope; 
        [SerializeField] private SlideConfig _slide;
        [SerializeField] private LifeCycleConfig _lifeCycle;

        [Header("Throwables")]
        [SerializeField] private ThrowableSettings _throwables;

        public MovementAbilityConfig Movement => _movement;
        public JumpAbilityConfig Jump => _jump;
        public DashAbilityConfig Dash => _dash;
        public GlideAbilityConfig Glide => _glide;
        public WallAbilityConfig Wall => _wall;
        public CombatAbilityConfig Combat => _combat;

        public SlopeConfig Slope => _slope;
        public SlideConfig Slide => _slide;
        public LifeCycleConfig LifeCycle => _lifeCycle;
        public ThrowableSettings Throwables => _throwables;
    }
}