using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.AudioManagment
{
    public interface IAudioService
    {
        void PlaySfx(string key);
        void PlaySfx(string key, Vector3? position = null);

        // Зацикленные эффекты (Добавлено)
        void PlaySfxLoop(string key, Vector3? position = null);
        void StopSfx(string key);

        // Одиночные треки
        void PlayMusic(string key, bool fade = true);

        // Плейлисты (Добавлено)
        void PlayPlaylist(string key, bool fade = true);

        // Управление громкостью
        void SetVolume(string parameterName, float value);
        float GetVolume(string parameterName);

        void SetMasterVolume(float value);
        void SetMusicVolume(float value);
        void SetSfxVolume(float value);

        float GetMasterVolume();
        float GetMusicVolume();
        float GetSfxVolume();
    }
}