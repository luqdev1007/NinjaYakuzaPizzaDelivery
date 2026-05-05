using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class StyleEvaluator
    {
        private readonly RankStyleService _styleService;
        private readonly StyleActionsConfig _config;
        private const int MaxHistorySize = 3;

        public StyleEvaluator(RankStyleService styleService, StyleActionsConfig config)
        {
            _styleService = styleService;
            _config = config;
        }

        public void ProcessDamage(EntitiesCore.Entity entity, float damage, string attackId)
        {
            float finalPoints = damage * _config.DamagePointMultiplier;
            var history = entity.GetComponent<MoveFreshness>().LastUsedTimes;

            if (!history.ContainsKey(attackId))
            {
                finalPoints *= _config.FreshnessBonus;
                history.Add(attackId, UnityEngine.Time.time);
                // Простая очистка старых записей если нужно
                if (history.Count > MaxHistorySize) history.Clear();
            }

            _styleService.AddPoints(entity, finalPoints);
        }

        public void ProcessDash(EntitiesCore.Entity entity) => _styleService.AddPoints(entity, _config.DashPoints);
        public void ProcessPlayerHit(EntitiesCore.Entity entity) => _styleService.ApplyDamagePenalty(entity);
        public void ProcessLoot(EntitiesCore.Entity entity) => _styleService.AddPoints(entity, _config.LootPickupPoints);
    }
}