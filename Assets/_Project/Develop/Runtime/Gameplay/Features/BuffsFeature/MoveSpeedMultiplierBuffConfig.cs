using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Buffs/Move Speed Multiplier Buff", fileName = "New Move Speed Multiplier Buff Config")]
    public class MoveSpeedMultiplierBuffConfig : BuffConfig
    {
        [field: SerializeField] public float Multiplier { get; private set; } = 2f;

        public override IBuffEffect CreateEffect()
        {
            return new MoveSpeedMultiplierBuffEffect(Multiplier);
        }
    }
}