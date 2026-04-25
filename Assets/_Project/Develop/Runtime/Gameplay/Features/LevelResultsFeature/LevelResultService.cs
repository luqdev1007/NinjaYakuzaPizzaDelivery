using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature
{
    public class LevelResultReport
    {
        public bool TimeStarEarned;
        public bool StyleStarEarned;
        public bool SecretStarEarned;
        public float FinalTime;
        public float FinalStylePoints;
        public string StyleLetter;
        public int CollectedSecrets;
        public int TotalSecrets;
    }

    public class LevelResultService
    {
        private readonly RankStyleService _styleService;
        private readonly SecretChestCollectService _secretChestCollectService;

        public LevelResultService(RankStyleService styleService, SecretChestCollectService secretChestCollectService)
        {
            _styleService = styleService;
            _secretChestCollectService = secretChestCollectService;
        }

        public LevelResultReport CalculateResult(LevelConfig config, float timeSpent)
        {
            return new LevelResultReport
            {
                FinalTime = timeSpent,
                FinalStylePoints = _styleService.MaxPoints,
                StyleLetter = _styleService.MaxLetter,

                TimeStarEarned = timeSpent <= config.TargetTime,
                StyleStarEarned = _styleService.MaxPoints >= config.StyleStarThreshold,

                SecretStarEarned = _secretChestCollectService.AllChestsCollected(),
                CollectedSecrets = _secretChestCollectService.CollectedCount,
                TotalSecrets = _secretChestCollectService.TotalCount
            };
        }
    }
}