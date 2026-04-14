using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class StyleEvaluator : IDisposable
    {
        private readonly RankStyleService _styleService;
        private readonly StyleActionsConfig _config;
        private readonly List<string> _usedActionsHistory = new();

        private const int MaxHistorySize = 3;

        public StyleEvaluator(RankStyleService styleService, StyleActionsConfig config)
        {
            _styleService = styleService;
            _config = config;
        }

        public void ProcessKill()
        {
            _styleService.AddPoints(_config.KillBasePoints);
        }

        public void ProcessDamage(float damage, string attackId)
        {
            float basePoints = damage * _config.DamagePointMultiplier;
            float finalPoints = basePoints;

            if (!_usedActionsHistory.Contains(attackId))
            {
                finalPoints *= _config.FreshnessBonus;

                _usedActionsHistory.Add(attackId);
                if (_usedActionsHistory.Count > MaxHistorySize)
                    _usedActionsHistory.RemoveAt(0);
            }

            _styleService.AddPoints(finalPoints);
        }

        public void ProcessDash()
        {
            _styleService.AddPoints(_config.DashPoints);
        }

        public void ProcessMovementAcceleration(float deltaTime)
        {
            float points = _config.UpwardAccelerationPoints * deltaTime;
            _styleService.AddPoints(points);
        }

        public void ProcessCoinCollect()
        {
            _styleService.AddPoints(_config.CoinCollectPoints);
        }

        public void ProcessMemoryFragmentCollect()
        {
            _styleService.AddPoints(_config.MemoryFragmentPoints);
        }

        public void ProcessPlayerHit()
        {
            _styleService.ApplyDamagePenalty();
        }

        public void Dispose()
        {
            _usedActionsHistory.Clear();
        }

        public void ProcessLootPick()
        {
            _styleService.AddPoints(_config.LootPickupPoints);
        }
    }
}
