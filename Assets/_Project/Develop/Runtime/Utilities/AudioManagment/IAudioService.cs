using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.AudioManagment
{
    public interface IAudioService
    {
        void PlaySfx(string key);
        void PlayMusic(string key, bool fade = true);
        void SetVolume(string parameterName, float value);
        void PlaySfx(string key, Vector3? position = null);
    }
}
