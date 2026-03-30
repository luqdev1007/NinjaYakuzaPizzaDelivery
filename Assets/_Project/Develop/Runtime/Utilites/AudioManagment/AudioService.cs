using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioService
    {
        private readonly AudioConfig _config;
        private readonly AudioManager _manager;

        public AudioService(AudioConfig config, AudioManager manager)
        {
            _config = config;
            _manager = manager;
        }

        public void PlayRandomSfx(AudioCategoryType category, bool useRandomPitch = true)
        {
            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            float pitch = data.BasePitch;

            if (useRandomPitch)
                pitch *= Random.Range(0.85f, 1.15f);

            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        public void PlaySfxById(string id, bool useRandomPitch = false)
        {
            var data = _config.GetById(id);
            if (data == null) return;

            float pitch = data.BasePitch;

            if (useRandomPitch)
                pitch *= Random.Range(0.9f, 1.1f);

            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        public void PlayMusic(string id)
        {
            var data = _config.GetById(id);
            if (data == null) return;

            _manager.PlayMusic(data.Clip, data.Volume);
        }
    }
}