using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
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
        private readonly Entity _heroEntity;
        private readonly SecretChestCollectService _secretChestCollectService;

        public LevelResultService(Entity heroEntity, SecretChestCollectService secretChestCollectService)
        {
            _heroEntity = heroEntity;
            _secretChestCollectService = secretChestCollectService;
        }

        public LevelResultReport CalculateResult(LevelConfig config, float timeSpent)
        {
            float maxPoints = _heroEntity.GetComponent<MaxStylePoints>().Value;
            StyleRankEnum maxRank = _heroEntity.GetComponent<MaxStyleRank>().Value;

            return new LevelResultReport
            {
                FinalTime = timeSpent,
                FinalStylePoints = maxPoints,
                StyleLetter = maxRank.ToString(), 

                TimeStarEarned = timeSpent <= config.TargetTime,
                StyleStarEarned = maxPoints >= config.StyleStarThreshold,

                SecretStarEarned = _secretChestCollectService.AllChestsCollected(),
                CollectedSecrets = _secretChestCollectService.CollectedCount,
                TotalSecrets = _secretChestCollectService.TotalCount
            };
        }
    }
}