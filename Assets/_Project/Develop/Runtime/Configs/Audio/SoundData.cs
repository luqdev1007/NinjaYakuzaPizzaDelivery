using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Configs/Audio/SoundData")]
    public class SoundData : AudioData
    {
        public AudioClip[] Clips; // Массив для рандомизации
        [Range(0.1f, 2f)] public float PitchMin = 0.9f;
        [Range(0.1f, 2f)] public float PitchMax = 1.1f;
    }
}