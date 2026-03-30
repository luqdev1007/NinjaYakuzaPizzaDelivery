using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;

        public void PlayMusic(AudioClip clip, float volume)
        {
            _musicSource.clip = clip;
            _musicSource.volume = volume;
            _musicSource.outputAudioMixerGroup = _musicGroup;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        public void PlaySfx(AudioClip clip, float volume, float pitch)
        {
            GameObject sfxObject = new GameObject($"SFX_{clip.name}");
            AudioSource source = sfxObject.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.outputAudioMixerGroup = _sfxGroup;

            source.Play();
            Destroy(sfxObject, (clip.length / pitch) + 0.2f);
        }
    }
}