using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSourceA;
        [SerializeField] private AudioSource _musicSourceB;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private int _maxSfxSources = 12;
        [SerializeField] private float _transitionDuration = 1.0f;

        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
        private AudioSource _activeMusicSource;

        private void Awake()
        {
            _activeMusicSource = _musicSourceA;
        }

        public void PlayMusic(AudioClip clip, float volume)
        {
            if (_activeMusicSource.clip == clip && _activeMusicSource.isPlaying) return;

            AudioSource inactiveSource = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

            StopAllCoroutines();
            StartCoroutine(FadeMusic(inactiveSource, clip, volume));
        }

        private IEnumerator FadeMusic(AudioSource newSource, AudioClip clip, float targetVolume)
        {
            newSource.clip = clip;
            newSource.volume = 0;
            newSource.loop = true;
            newSource.outputAudioMixerGroup = _musicGroup;
            newSource.Play();

            float startActiveVol = _activeMusicSource.volume;
            float timer = 0;

            while (timer < _transitionDuration)
            {
                timer += Time.deltaTime;
                float percent = timer / _transitionDuration;

                _activeMusicSource.volume = Mathf.Lerp(startActiveVol, 0, percent);
                newSource.volume = Mathf.Lerp(0, targetVolume, percent);
                yield return null;
            }

            _activeMusicSource.Stop();
            _activeMusicSource = newSource;
        }

        public void PlaySfx(AudioClip clip, float volume, float pitch)
        {
            AudioSource source = GetFreeSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.outputAudioMixerGroup = _sfxGroup;
            source.Play();
        }

        private AudioSource GetFreeSource()
        {
            for (int i = 0; i < _sfxPool.Count; i++)
            {
                if (!_sfxPool[i].isPlaying) return _sfxPool[i];
            }

            if (_sfxPool.Count < _maxSfxSources)
            {
                GameObject obj = new GameObject($"SFX_Source_{_sfxPool.Count}");
                obj.transform.SetParent(transform);
                AudioSource newSource = obj.AddComponent<AudioSource>();
                _sfxPool.Add(newSource);
                return newSource;
            }

            return null;
        }
    }
}