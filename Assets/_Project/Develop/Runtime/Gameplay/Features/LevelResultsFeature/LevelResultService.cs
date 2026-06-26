using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature
{
    public class LevelResultReport
    {
        public bool TimeStarEarned;
        public bool StyleStarEarned;
        public bool CurrencyStarEarned;

        public float FinalTime;
        public float FinalStylePoints;
        public string StyleLetter;

        public int CollectedGold;
        public int CollectedShards;
        public int GoldThreshold;
        public int ShardThreshold;
    }

    public class LevelResultService
    {
        private readonly RankStyleService _styleService;
        private readonly SessionLootService _sessionLootService;

        public LevelResultService(RankStyleService styleService, SessionLootService sessionLootService)
        {
            _styleService = styleService;
            _sessionLootService = sessionLootService;
        }

        public LevelResultReport CalculateResult(LevelConfig config, float timeSpent)
        {
            int collectedGold = _sessionLootService.GetCollectedAmount(LootTypes.Coin);
            int collectedShards = _sessionLootService.GetCollectedAmount(LootTypes.SoulShard);

            bool currencyStarEarned =
                collectedGold >= config.CurrencyStarGoldThreshold &&
                collectedShards >= config.CurrencyStarShardThreshold;

            return new LevelResultReport
            {
                FinalTime = timeSpent,
                FinalStylePoints = _styleService.MaxPoints,
                StyleLetter = _styleService.MaxLetter,

                TimeStarEarned = timeSpent <= config.TargetTime,
                StyleStarEarned = _styleService.MaxPoints >= config.StyleStarThreshold,

                CurrencyStarEarned = currencyStarEarned,
                CollectedGold = collectedGold,
                CollectedShards = collectedShards,
                GoldThreshold = config.CurrencyStarGoldThreshold,
                ShardThreshold = config.CurrencyStarShardThreshold
            };
        }
    }
}