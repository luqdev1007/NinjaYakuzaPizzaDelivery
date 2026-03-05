using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Stats
{
    [CreateAssetMenu(menuName = "Configs/Stats/New Game Rewards Config", fileName = "GameRewardsConfig", order = 54)]
    public class GameRewardsConfig : ScriptableObject
    {
        [field: SerializeField] public int RewardForWin { get; private set; } = 10;
        [field: SerializeField] public int PenaltyForLose { get; private set; } = 5;
        [field: SerializeField] public int ResetCost { get; private set; } = 20;
    }
}