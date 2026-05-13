using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
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

        // Поля для хранения максимумов за уровень
        private float _maxPoints;
        private string _maxLetter = "F";

        public IReadOnlyVariable<float> CurrentPoints => _currentPoints;
        public IReadOnlyVariable<string> CurrentLetter => _currentLetter;
        public IReadOnlyVariable<string> CurrentPrefix => _currentPrefix;

        // Публичные свойства для получения рекордов в конце уровня
        public float MaxPoints => _maxPoints;
        public string MaxLetter => _maxLetter;

        public RankStyleService(StyleRankConfig rankConfig, StyleActionsConfig actionsConfig)
        {
            _rankConfig = rankConfig;
            _actionsConfig = actionsConfig;
        }

        private float _lastGainTime;
        private const float DecayDelay = 3f;

        public void AddPoints(float amount)
        {
            if (amount <= 0) return;

            _lastGainTime = Time.time;

            float multiplier = GetCurrentMultiplier();
            _currentPoints.Value += amount * multiplier;

            // Запоминаем максимальное количество очков
            if (_currentPoints.Value > _maxPoints)
            {
                _maxPoints = _currentPoints.Value;
            }

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
            string bestLetter = _rankConfig.Ranks[0].Letter;
            string bestPrefix = _rankConfig.Ranks[0].SubRanks[0].Prefix;

            foreach (var rankEntry in _rankConfig.Ranks)
            {
                foreach (var subRank in rankEntry.SubRanks)
                {
                    if (_currentPoints.Value >= subRank.Threshold)
                    {
                        bestLetter = rankEntry.Letter;
                        bestPrefix = subRank.Prefix;
                    }
                    else
                    {
                        goto Assign;
                    }
                }
            }

        Assign:
            _currentLetter.Value = bestLetter;
            _currentPrefix.Value = bestPrefix;

            // Обновляем максимальный достигнутый ранг (букву)
            UpdateMaxLetter(bestLetter);
        }

        private void UpdateMaxLetter(string currentLetter)
        {
            // Находим индекс текущего и максимального ранга в конфиге, чтобы сравнить их
            int currentIndex = _rankConfig.Ranks.FindIndex(r => r.Letter == currentLetter);
            int maxIndex = _rankConfig.Ranks.FindIndex(r => r.Letter == _maxLetter);

            if (currentIndex > maxIndex)
            {
                _maxLetter = currentLetter;
            }
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
            _maxPoints = 0f;
            _maxLetter = "F";
            _lastGainTime = 0f;
            UpdateRank();
        }
    }
}