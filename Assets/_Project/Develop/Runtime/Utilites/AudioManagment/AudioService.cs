using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioService
    {
        private readonly AudioConfig _config;
        private readonly AudioManager _manager;
        private readonly Dictionary<string, float> _lastPlayedTimes = new Dictionary<string, float>();

        private MusicPlaylist _currentPlaylist;
        private int _lastTrackIndex = -1;
        private const float GlobalSfxCooldown = 0.05f;

        public AudioService(AudioConfig config, AudioManager manager)
        {
            _config = config;
            _manager = manager;
            _manager.OnMusicEnded += PlayNextFromPlaylist;
        }

        // Тот самый метод для систем атаки
        public void PlayRandomSfx(AudioCategoryType category, bool useRandomPitch = true)
        {
            // Используем имя категории как ключ для кулдауна
            if (IsSpamming(category.ToString())) return;

            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            _lastPlayedTimes[category.ToString()] = Time.time;
            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        public void PlaySfxByPrefix(string prefix, bool useRandomPitch = true)
        {
            if (IsSpamming(prefix)) return;

            var data = _config.GetRandomByPrefix(prefix);
            if (data == null) return;

            _lastPlayedTimes[prefix] = Time.time;
            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        public void StartPlaylist(string playlistId)
        {
            _currentPlaylist = _config.GetPlaylist(playlistId);
            _lastTrackIndex = -1;
            PlayNextFromPlaylist();
        }

        private void PlayNextFromPlaylist()
        {
            if (_currentPlaylist == null || _currentPlaylist.Tracks.Count == 0) return;

            int nextIndex;
            if (_currentPlaylist.Tracks.Count == 1) nextIndex = 0;
            else
            {
                do { nextIndex = Random.Range(0, _currentPlaylist.Tracks.Count); }
                while (nextIndex == _lastTrackIndex);
            }

            _lastTrackIndex = nextIndex;
            _manager.PlayMusic(_currentPlaylist.Tracks[nextIndex], _currentPlaylist.Volume, false);
        }

        private bool IsSpamming(string id)
        {
            if (_lastPlayedTimes.TryGetValue(id, out float lastTime))
                return (Time.time - lastTime) < GlobalSfxCooldown;
            return false;
        }
    }
}