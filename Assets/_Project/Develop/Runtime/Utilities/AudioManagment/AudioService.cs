using Assets._Project.Develop.Runtime.Configs.Audio;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ObjectsManagment;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilities.AudioManagment
{
    public class AudioService : IAudioService
    {
        private readonly AudioLibrary _library;
        private readonly AudioMixer _mixer;
        private readonly GameObjectPool _sfxPool;
        private readonly ICoroutinesPerformer _coroutines;

        private AudioSource _activeMusicSource;
        private AudioSource _inactiveMusicSource;
        private Coroutine _fadeCoroutine;

        public AudioService(AudioLibrary library, AudioMixer mixer, GameObjectPool sfxPool, ICoroutinesPerformer coroutines)
        {
            _library = library;
            _mixer = mixer;
            _sfxPool = sfxPool;
            _coroutines = coroutines;

            _library.Initialize();
            CreateMusicSources();
        }

        private void CreateMusicSources()
        {
            _activeMusicSource = new GameObject("MusicSource_Active").AddComponent<AudioSource>();
            _inactiveMusicSource = new GameObject("MusicSource_Inactive").AddComponent<AudioSource>();

            Object.DontDestroyOnLoad(_activeMusicSource.gameObject);
            Object.DontDestroyOnLoad(_inactiveMusicSource.gameObject);
        }

        public void PlaySfx(string key) => PlaySfx(key, null);

        public void PlaySfx(string key, Vector3? position = null)
        {
            var data = _library.GetSound(key);
            if (data == null) return;

            var emitterObj = _sfxPool.Get();
            var emitter = emitterObj.GetComponent<AudioEmitter>();

            emitter.Play(data, position, (e) => _sfxPool.Return(e.gameObject));
        }

        public void PlayMusic(string key, bool fade = true)
        {
            var data = _library.GetMusic(key);

            if (data == null || (_activeMusicSource.clip == data.Clip && _activeMusicSource.isPlaying))
                return;

            if (_fadeCoroutine != null)
                _coroutines.StopPerform(_fadeCoroutine);

            if (fade)
                _fadeCoroutine = _coroutines.StartPerform(CrossfadeCoroutine(data));
            else
                SwitchMusicImmediately(data);
        }

        public void SetVolume(string parameterName, float value)
        {
            float db = value > 0 ? Mathf.Log10(value) * 20 : -80f;
            _mixer.SetFloat(parameterName, db);
        }

        private void SwitchMusicImmediately(MusicData data)
        {
            _activeMusicSource.Stop();
            _activeMusicSource.clip = data.Clip;
            _activeMusicSource.volume = data.Volume;
            _activeMusicSource.outputAudioMixerGroup = data.Group;
            _activeMusicSource.loop = true;
            _activeMusicSource.Play();
        }

        private IEnumerator CrossfadeCoroutine(MusicData data)
        {
            float duration = data.FadeDuration;
            float elapsed = 0;

            _inactiveMusicSource.clip = data.Clip;
            _inactiveMusicSource.volume = 0;
            _inactiveMusicSource.outputAudioMixerGroup = data.Group;
            _inactiveMusicSource.loop = true;
            _inactiveMusicSource.Play();

            float startVolume = _activeMusicSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                _activeMusicSource.volume = Mathf.Lerp(startVolume, 0, t);
                _inactiveMusicSource.volume = Mathf.Lerp(0, data.Volume, t);
                yield return null;
            }

            _activeMusicSource.Stop();

            var temp = _activeMusicSource;
            _activeMusicSource = _inactiveMusicSource;
            _inactiveMusicSource = temp;

            _fadeCoroutine = null;
        }
    }
}