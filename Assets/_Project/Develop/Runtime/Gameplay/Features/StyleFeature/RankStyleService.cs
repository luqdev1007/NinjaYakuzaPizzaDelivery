using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using UnityEngine;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class RankStyleService
    {
        private readonly StyleRankConfig _rankConfig;
        private readonly StyleActionsConfig _actionsConfig;
        private readonly List<SubRankEntry> _flattenedSubRanks = new();

        public RankStyleService(StyleRankConfig rankConfig, StyleActionsConfig actionsConfig)
        {
            _rankConfig = rankConfig;
            _actionsConfig = actionsConfig;

            foreach (var rank in _rankConfig.Ranks)
                foreach (var subRank in rank.SubRanks)
                    _flattenedSubRanks.Add(subRank);
        }

        public void AddPoints(EntitiesCore.Entity entity, float amount)
        {
            if (amount <= 0) 
                return;

            entity.GetComponent<StyleDecayTimer>().Value.Value = 0f;
            float multiplier = entity.GetComponent<StyleMultiplier>().Value.Value;

            var pointsComp = entity.GetComponent<StylePoints>().Value;
            pointsComp.Value += amount * multiplier;

            var maxPointsComp = entity.GetComponent<MaxStylePoints>();

            if (pointsComp.Value > maxPointsComp.Value)
            {
                maxPointsComp.Value = pointsComp.Value;
            }

            UpdateRank(entity);
        }

        public void ApplyDamagePenalty(EntitiesCore.Entity entity)
        {
            float points = entity.GetComponent<StylePoints>().Value.Value;
            int currentIndex = GetSubRankIndex(points);
            int penalty = _actionsConfig.RanksToDropOnDamage;
            int newIndex = Mathf.Max(0, currentIndex - penalty);

            entity.GetComponent<StylePoints>().Value.Value = _flattenedSubRanks[newIndex].Threshold;
            UpdateRank(entity);
        }

        public void UpdateDecay(EntitiesCore.Entity entity, float deltaTime)
        {
            var timer = entity.GetComponent<StyleDecayTimer>().Value;
            var points = entity.GetComponent<StylePoints>().Value;

            timer.Value += deltaTime;

            if (timer.Value < 3f || points.Value <= 0) 
                return;

            float decayRate = GetDecayRate(points.Value);
            float amountToRemove = decayRate * _rankConfig.GlobalDecayMultiplier * deltaTime;

            points.Value = Mathf.Max(0, points.Value - amountToRemove);
            UpdateRank(entity);
        }

        private void UpdateRank(EntitiesCore.Entity entity)
        {
            float points = entity.GetComponent<StylePoints>().Value.Value;
            int index = GetSubRankIndex(points);

            entity.GetComponent<StyleMultiplier>().Value.Value = _flattenedSubRanks[index].Multiplier;

            StyleRankEnum newRank = DetermineRankEnum(points);
            entity.GetComponent<StyleRank>().Value.Value = newRank;

            var maxRankComp = entity.GetComponent<MaxStyleRank>();
            if ((int)newRank > (int)maxRankComp.Value)
            {
                maxRankComp.Value = newRank;
            }
        }

        private int GetSubRankIndex(float points)
        {
            int index = 0;
            for (int i = 0; i < _flattenedSubRanks.Count; i++)
            {
                if (points >= _flattenedSubRanks[i].Threshold) index = i;
                else break;
            }
            return index;
        }

        private float GetDecayRate(float points)
        {
            float rate = _rankConfig.Ranks[0].DecayRate;
            foreach (var r in _rankConfig.Ranks)
            {
                if (points >= r.SubRanks[0].Threshold) rate = r.DecayRate;
                else break;
            }
            return rate;
        }

        private StyleRankEnum DetermineRankEnum(float points)
        {
            StyleRankEnum rank = StyleRankEnum.F;
            for (int i = 0; i < _rankConfig.Ranks.Count; i++)
            {
                if (points >= _rankConfig.Ranks[i].SubRanks[0].Threshold)
                    rank = (StyleRankEnum)(i + 1); // +1 так как F — это 0
            }
            return rank;
        }
    }
}