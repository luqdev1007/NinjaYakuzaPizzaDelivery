using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    [CreateAssetMenu(fileName = "StyleRankConfig", menuName = "Configs/Gameplay/Style/Style Rank Config")]
    public class StyleRankConfig : ScriptableObject
    {
        [Header("Settings")]
        public float GlobalDecayMultiplier = 1f; // Общая скорость затухания

        [Header("Ranks")]
        public List<MainRankEntry> Ranks;
    }

    [Serializable]
    public class MainRankEntry
    {
        public string Letter; // F, C, B, A, S
        public List<SubRankEntry> SubRanks;

        // Как быстро сгорают очки именно на этом ранге
        public float DecayRate = 10f;
    }

    [Serializable]
    public class SubRankEntry
    {
        public string Prefix;    // "xurslf", "oo1", "mokin'"
        public float Threshold;  // Сколько очков нужно набрать для этого под-ранга
        public float Multiplier; // x1.5, x2.0 и т.д.
    }
}
