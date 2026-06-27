using Assets._Project.Develop.Runtime.Configs.Gameplay.Buffs;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Buffs/Loot Collect Range Additive Buff", fileName = "New Loot Collect Range Additive Buff Config")]
    public class LootCollectRangeAdditiveBuffConfig : BuffConfig
    {
        [field: SerializeField] public float AdditiveAmount { get; private set; } = 10f;

        public override IBuffEffect CreateEffect()
        {
            return new LootCollectRangeAdditiveBuffEffect(AdditiveAmount);
        }
    }
}