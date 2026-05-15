using System.Collections.Generic;
using UnityEngine;
using AudioSettings = Assets._Project.Develop.Runtime.Utilities.AudioManagment.AudioSettings;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Configs/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public AudioSettings Settings;
        public List<SoundData> Sounds;
        public List<MusicData> Musics;
        public List<PlaylistData> Playlists; // Новый список плейлистов

        private Dictionary<string, SoundData> _soundCache;
        private Dictionary<string, MusicData> _musicCache;
        private Dictionary<string, PlaylistData> _playlistCache;

        public void Initialize()
        {
            _soundCache = new Dictionary<string, SoundData>();
            foreach (var s in Sounds)
            {
                if (s != null && !string.IsNullOrEmpty(s.Key))
                    _soundCache[s.Key] = s;
            }

            _musicCache = new Dictionary<string, MusicData>();
            foreach (var m in Musics)
            {
                if (m != null && !string.IsNullOrEmpty(m.Key))
                    _musicCache[m.Key] = m;
            }

            _playlistCache = new Dictionary<string, PlaylistData>();
            foreach (var p in Playlists)
            {
                if (p != null && !string.IsNullOrEmpty(p.Key))
                    _playlistCache[p.Key] = p;
            }
        }

        public SoundData GetSound(string key)
        {
            if (_soundCache == null) Initialize();
            return _soundCache.GetValueOrDefault(key);
        }

        public MusicData GetMusic(string key)
        {
            if (_musicCache == null) Initialize();
            return _musicCache.GetValueOrDefault(key);
        }

        public PlaylistData GetPlaylist(string key)
        {
            if (_playlistCache == null) Initialize();
            return _playlistCache.GetValueOrDefault(key);
        }
    }
}