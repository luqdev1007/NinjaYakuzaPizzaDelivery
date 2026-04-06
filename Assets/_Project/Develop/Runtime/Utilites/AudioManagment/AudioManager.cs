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
        [SerializeField] private AudioMixerGroup _uiGroup;
        [SerializeField] private int _maxSfxSources = 32;
        [SerializeField] private float _transitionDuration = 1.2f;

        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
        private AudioSource _activeMusicSource;

        // Это свойство нужно для регистрации в DI
        public AudioMixer MainMixer => _musicGroup.audioMixer;

        public event Action OnMusicEnded;

        private void Awake() => _activeMusicSource = _musicSourceA;

        private void Update()
        {
            if (_activeMusicSource.clip != null && !_activeMusicSource.isPlaying && !_activeMusicSource.loop)
            {
                _activeMusicSource.clip = null;
                OnMusicEnded?.Invoke();
            }
        }

        public void PlayMusic(AudioClip clip, float volume, bool loop = true)
        {
            if (_activeMusicSource.clip == clip) return;
            AudioSource target = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;
            if (_musicFadeCoroutine != null) StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = StartCoroutine(FadeMusic(target, clip, volume, loop));
        }

        private Coroutine _musicFadeCoroutine;
        private IEnumerator FadeMusic(AudioSource newSource, AudioClip clip, float targetVol, bool loop)
        {
            newSource.clip = clip;
            newSource.volume = 0;
            newSource.loop = loop;
            newSource.outputAudioMixerGroup = _musicGroup;
            newSource.Play();

            float startVol = _activeMusicSource.volume;
            float timer = 0;
            while (timer < _transitionDuration)
            {
                timer += Time.deltaTime;
                float p = timer / _transitionDuration;
                _activeMusicSource.volume = Mathf.Lerp(startVol, 0, p);
                newSource.volume = Mathf.Lerp(0, targetVol, p);
                yield return null;
            }
            _activeMusicSource.Stop();
            _activeMusicSource = newSource;
        }

        public void PlaySfx(AudioClip clip, float vol, float pitch, bool isUi = false)
            => PlaySfxReturnSource(clip, vol, pitch, isUi);

        public AudioSource PlaySfxReturnSource(AudioClip clip, float vol, float pitch, bool isUi = false)
        {
            AudioSource source = GetFreeSource();
            if (source == null) return null;

            source.clip = clip;
            source.volume = vol;
            source.pitch = pitch;
            source.outputAudioMixerGroup = isUi ? _uiGroup : _sfxGroup;
            source.Play();
            return source;
        }

        private AudioSource GetFreeSource()
        {
            for (int i = 0; i < _sfxPool.Count; i++)
                if (!_sfxPool[i].isPlaying) return _sfxPool[i];

            if (_sfxPool.Count < _maxSfxSources)
            {
                GameObject obj = new GameObject($"SFX_{_sfxPool.Count}");
                obj.transform.SetParent(transform);
                AudioSource s = obj.AddComponent<AudioSource>();
                s.playOnAwake = false;
                s.spatialBlend = 0f;
                _sfxPool.Add(s);
                return s;
            }
            return null;
        }
    }
}