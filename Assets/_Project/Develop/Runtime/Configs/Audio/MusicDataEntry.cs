using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [Serializable]
    public class MusicDataEntry
    {
        public AudioClip Clip;
        [Range(0, 1)] public float Volume = 1f;
    }
}