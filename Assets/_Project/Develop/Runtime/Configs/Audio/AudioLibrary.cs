using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Configs/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public AudioSettings Settings;
        public List<SoundData> Sounds;
        public List<MusicData> Musics;

        private Dictionary<string, SoundData> _soundCache;
        private Dictionary<string, MusicData> _musicCache;

        public void Initialize()
        {
            _soundCache = new Dictionary<string, SoundData>();

            foreach (var s in Sounds)
                _soundCache[s.Key] = s;

            _musicCache = new Dictionary<string, MusicData>();
            foreach (var m in Musics) 
                _musicCache[m.Key] = m;
        }

        public SoundData GetSound(string key) => _soundCache.GetValueOrDefault(key);
        public MusicData GetMusic(string key) => _musicCache.GetValueOrDefault(key);
    }
}
