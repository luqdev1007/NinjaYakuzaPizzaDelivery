using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    public abstract class AudioData : ScriptableObject
    {
        public string Key;
        [Range(0, 1)] public float Volume = 1f;
    }

    [CreateAssetMenu(fileName = "SoundData", menuName = "Configs/Audio/SoundData")]
    public class SoundData : AudioData
    {
        public AudioClip[] Clips;
        [Range(0.1f, 2f)] public float PitchMin = 0.9f;
        [Range(0.1f, 2f)] public float PitchMax = 1.1f;
        public AudioMixerGroup Group;
    }

    [CreateAssetMenu(fileName = "MusicData", menuName = "Configs/Audio/MusicData")]
    public class MusicData : AudioData
    {
        public AudioClip Clip;
        public float FadeDuration = 1.5f;
        public AudioMixerGroup Group;
    }
}
