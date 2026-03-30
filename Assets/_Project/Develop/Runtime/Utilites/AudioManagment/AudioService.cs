using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioService
    {
        private readonly AudioConfig _config;
        private readonly AudioManager _manager;
        private readonly Dictionary<string, float> _lastPlayedTimes = new Dictionary<string, float>();

        private const float GlobalSfxCooldown = 0.05f;

        public AudioService(AudioConfig config, AudioManager manager)
        {
            _config = config;
            _manager = manager;
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

        public void PlayMusic(string id)
        {
            var data = _config.GetById(id);
            if (data == null) return;

            _manager.PlayMusic(data.Clip, data.Volume);
        }

        private bool IsSpamming(string id)
        {
            if (_lastPlayedTimes.TryGetValue(id, out float lastTime))
            {
                return (Time.time - lastTime) < GlobalSfxCooldown;
            }
            return false;
        }

        public void PlayRandomSfx(AudioCategoryType category, bool useRandomPitch = true)
        {
            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.85f, 1.15f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }
    }
}