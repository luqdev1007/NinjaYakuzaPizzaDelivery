using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Configs/Audio/MusicData")]
    public class MusicData : AudioData
    {
        public AudioClip Clip;
        public float FadeDuration = 1.5f;
    }
}