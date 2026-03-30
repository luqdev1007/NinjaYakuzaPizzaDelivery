using System;
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
        [SerializeField] private int _maxSfxSources = 32;
        [SerializeField] private float _transitionDuration = 1.2f;

        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
        private AudioSource _activeMusicSource;

        public AudioMixer MusicMixer => _musicGroup.audioMixer;

        public event Action OnMusicEnded;

        private void Awake() => _activeMusicSource = _musicSourceA;

        private void Update()
        {
            if (_activeMusicSource.clip != null && !_activeMusicSource.isPlaying && _activeMusicSource.loop == false)
            {
                _activeMusicSource.clip = null; // Чтобы не вызывать событие каждый кадр
                OnMusicEnded?.Invoke();
            }
        }

        private Coroutine _musicFadeCoroutine;

        public void PlayMusic(AudioClip clip, float volume, bool loop = true)
        {
            if (_activeMusicSource.clip == clip) return;
            AudioSource inactiveSource = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

            if (_musicFadeCoroutine != null) StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = StartCoroutine(FadeMusic(inactiveSource, clip, volume, loop));
        }

        private IEnumerator FadeMusic(AudioSource newSource, AudioClip clip, float targetVolume, bool loop)
        {
            newSource.clip = clip;
            newSource.volume = 0;
            newSource.loop = loop;
            newSource.outputAudioMixerGroup = _musicGroup;
            newSource.Play();

            float startActiveVol = _activeMusicSource.volume;
            float timer = 0;

            while (timer < _transitionDuration)
            {
                timer += Time.deltaTime;
                float p = timer / _transitionDuration;
                _activeMusicSource.volume = Mathf.Lerp(startActiveVol, 0, p);
                newSource.volume = Mathf.Lerp(0, targetVolume, p);
                yield return null;
            }

            _activeMusicSource.Stop();
            _activeMusicSource = newSource;
        }

        /// <summary>
        /// Проигрывает SFX без возврата ссылки.
        /// </summary>
        public void PlaySfx(AudioClip clip, float volume, float pitch)
        {
            PlaySfxReturnSource(clip, volume, pitch);
        }

        /// <summary>
        /// Проигрывает SFX и возвращает AudioSource для дальнейшего контроля (например, для зацикливания или остановки).
        /// </summary>
        public AudioSource PlaySfxReturnSource(AudioClip clip, float volume, float pitch)
        {
            AudioSource source = GetFreeSource();

            if (source == null)
                return null;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.outputAudioMixerGroup = _sfxGroup;
            source.Play();

            return source;
        }

        private AudioSource GetFreeSource()
        {
            // Ищем свободный источник в пуле
            for (int i = 0; i < _sfxPool.Count; i++)
            {
                if (!_sfxPool[i].isPlaying)
                    return _sfxPool[i];
            }

            // Если свободных нет и пул не переполнен — создаем новый
            if (_sfxPool.Count < _maxSfxSources)
            {
                GameObject obj = new GameObject($"SFX_Source_{_sfxPool.Count}");
                obj.transform.SetParent(transform);
                AudioSource newSource = obj.AddComponent<AudioSource>();

                // Настройки по умолчанию для 2D звука
                newSource.playOnAwake = false;
                newSource.spatialBlend = 0f;

                _sfxPool.Add(newSource);
                return newSource;
            }

            return null;
        }
    }
}