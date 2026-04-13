using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    [CreateAssetMenu(fileName = "StyleRankConfig", menuName = "Configs/Gameplay/Style/Style Rank Config")]
    public class StyleRankConfig : ScriptableObject
    {
        [Header("Settings")]
        [Tooltip("Общая скорость затухания очков стиля")]
        public float GlobalDecayMultiplier = 1f;

        [Header("Ranks")]
        public List<MainRankEntry> Ranks;
    }

    [Serializable]
    public class MainRankEntry
    {
        [Tooltip("Буквенное обозначение ранга: F, C, B, A, S")]
        public string Letter;

        public List<SubRankEntry> SubRanks;

        [Tooltip("Скорость сгорания очков именно на этом ранге")]
        public float DecayRate = 10f;
    }

    [Serializable]
    public class SubRankEntry
    {
        [Tooltip("Префикс для отображения: xurslf, oo1, mokin'")]
        public string Prefix;

        [Tooltip("Порог очков для перехода на этот под-ранг")]
        public float Threshold;

        [Tooltip("Множитель очков (x1.5, x2.0 и т.д.)")]
        public float Multiplier;
    }
}