using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class RankStyleService
    {
        private readonly StyleRankConfig _rankConfig;
        private readonly StyleActionsConfig _actionsConfig;

        private readonly ReactiveVariable<float> _currentPoints = new(0f);
        private readonly ReactiveVariable<string> _currentLetter = new("F");
        private readonly ReactiveVariable<string> _currentPrefix = new("");

        public IReadOnlyVariable<float> CurrentPoints => _currentPoints;
        public IReadOnlyVariable<string> CurrentLetter => _currentLetter;
        public IReadOnlyVariable<string> CurrentPrefix => _currentPrefix;

        public RankStyleService(StyleRankConfig rankConfig, StyleActionsConfig actionsConfig)
        {
            _rankConfig = rankConfig;
            _actionsConfig = actionsConfig;
        }

        private float _lastGainTime;
        private const float DecayDelay = 3f; // Задержка перед началом падения очков

        public void AddPoints(float amount)
        {
            if (amount <= 0) return;

            _lastGainTime = Time.time; // Запоминаем время последнего получения очков

            float multiplier = GetCurrentMultiplier();
            _currentPoints.Value += amount * multiplier;

            UpdateRank();
        }

        public void ApplyDamagePenalty()
        {
            var allSubRanks = GetFlattenedSubRanks();
            int currentSubRankIndex = 0;

            for (int i = 0; i < allSubRanks.Count; i++)
            {
                if (_currentPoints.Value >= allSubRanks[i].Threshold)
                    currentSubRankIndex = i;
                else
                    break;
            }

            int penalty = _actionsConfig.RanksToDropOnDamage;
            int newIndex = Mathf.Max(0, currentSubRankIndex - penalty);

            _currentPoints.Value = allSubRanks[newIndex].Threshold;

            UpdateRank();
        }

        public void UpdateDecay(float deltaTime)
        {
            if (Time.time - _lastGainTime < DecayDelay) 
                return;

            if (_currentPoints.Value <= 0) 
                return;

            float decayRate = GetCurrentDecayRate();
            float amountToRemove = decayRate * _rankConfig.GlobalDecayMultiplier * deltaTime;

            _currentPoints.Value = Mathf.Max(0, _currentPoints.Value - amountToRemove);
            UpdateRank();
        }

        private void UpdateRank()
        {
            // Берем САМЫЙ ПЕРВЫЙ ранг и подранг из конфига как дефолт
            // Чтобы даже при 0 очков у нас была буква "F"
            string bestLetter = _rankConfig.Ranks[0].Letter;
            string bestPrefix = _rankConfig.Ranks[0].SubRanks[0].Prefix;

            foreach (var rankEntry in _rankConfig.Ranks)
            {
                foreach (var subRank in rankEntry.SubRanks)
                {
                    // Используем >=, чтобы при равенстве порогу (включая 0) ранг находился
                    if (_currentPoints.Value >= subRank.Threshold)
                    {
                        bestLetter = rankEntry.Letter;
                        bestPrefix = subRank.Prefix;
                    }
                    else
                    {
                        // Как только нашли порог, который выше наших очков — выходим
                        goto Assign;
                    }
                }
            }

        Assign:
            _currentLetter.Value = bestLetter;
            _currentPrefix.Value = bestPrefix;
        }

        private float GetCurrentMultiplier()
        {
            float multiplier = 1f;
            foreach (var rankEntry in _rankConfig.Ranks)
            {
                foreach (var subRank in rankEntry.SubRanks)
                {
                    if (_currentPoints.Value >= subRank.Threshold)
                        multiplier = subRank.Multiplier;
                    else
                        return multiplier;
                }
            }
            return multiplier;
        }

        private float GetCurrentDecayRate()
        {
            float rate = _rankConfig.Ranks[0].DecayRate;
            foreach (var rankEntry in _rankConfig.Ranks)
            {
                if (_currentPoints.Value >= rankEntry.SubRanks[0].Threshold)
                    rate = rankEntry.DecayRate;
                else
                    break;
            }
            return rate;
        }

        private List<SubRankEntry> GetFlattenedSubRanks()
        {
            List<SubRankEntry> subRanks = new();
            foreach (var rank in _rankConfig.Ranks)
            {
                subRanks.AddRange(rank.SubRanks);
            }
            return subRanks;
        }

        public void Deactivate()
        {
            _currentPoints.Value = 0f;
            _lastGainTime = 0f;
            UpdateRank();
        }
    }
}


