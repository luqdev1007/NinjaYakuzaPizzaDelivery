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

        // Поля для плейлиста
        private PlaylistData _currentPlaylist;
        private int _currentTrackIndex;
        private Coroutine _playlistCoroutine;

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

        // SFX
        public void PlaySfx(string key) => PlaySfx(key, null);

        public void PlaySfx(string key, Vector3? position = null)
        {
            var data = _library.GetSound(key);
            if (data == null) return;

            var emitterObj = _sfxPool.Get();
            var emitter = emitterObj.GetComponent<AudioEmitter>();

            emitter.Play(data, position, (e) => _sfxPool.Return(e.gameObject));
        }

        // Плейлисты
        public void PlayPlaylist(string key, bool fade = true)
        {
            var playlist = _library.GetPlaylist(key);
            if (playlist == null || playlist.Tracks.Count == 0) return;

            if (_playlistCoroutine != null)
                _coroutines.StopPerform(_playlistCoroutine);

            _currentPlaylist = playlist;
            _currentTrackIndex = playlist.Shuffle ? Random.Range(0, playlist.Tracks.Count) : 0;

            _playlistCoroutine = _coroutines.StartPerform(PlaylistProcessor(fade));
        }

        private IEnumerator PlaylistProcessor(bool fade)
        {
            while (_currentPlaylist != null && _currentPlaylist.Tracks.Count > 0)
            {
                var trackEntry = _currentPlaylist.Tracks[_currentTrackIndex];

                // Создаем временную MusicData для работы кроссфейда
                MusicData tempMusic = ScriptableObject.CreateInstance<MusicData>();
                tempMusic.Clip = trackEntry.Clip;
                tempMusic.Volume = trackEntry.Volume;
                tempMusic.FadeDuration = 2.0f;
                tempMusic.Group = _mixer.FindMatchingGroups("Music")[0];

                PlayMusicInternal(tempMusic, fade);

                // Ждем окончания трека минус время следующего фейда
                yield return new WaitForSeconds(trackEntry.Clip.length - 1.5f);

                // Выбор следующего трека
                if (_currentPlaylist.Shuffle)
                    _currentTrackIndex = Random.Range(0, _currentPlaylist.Tracks.Count);
                else
                    _currentTrackIndex = (_currentTrackIndex + 1) % _currentPlaylist.Tracks.Count;
            }
        }

        // Одиночная музыка
        public void PlayMusic(string key, bool fade = true)
        {
            var data = _library.GetMusic(key);
            if (data == null) return;

            // Если включаем одиночный трек, останавливаем плейлист
            if (_playlistCoroutine != null)
            {
                _coroutines.StopPerform(_playlistCoroutine);
                _playlistCoroutine = null;
                _currentPlaylist = null;
            }

            PlayMusicInternal(data, fade);
        }

        private void PlayMusicInternal(MusicData data, bool fade)
        {
            if (data == null || (_activeMusicSource.clip == data.Clip && _activeMusicSource.isPlaying))
                return;

            if (_fadeCoroutine != null)
                _coroutines.StopPerform(_fadeCoroutine);

            if (fade)
                _fadeCoroutine = _coroutines.StartPerform(CrossfadeCoroutine(data));
            else
                SwitchMusicImmediately(data);
        }

        // Volume Management
        public void SetVolume(string parameterName, float value)
        {
            float db = value > 0.0001f ? Mathf.Log10(value) * 20 : -80f;
            _mixer.SetFloat(parameterName, db);
        }

        public float GetVolume(string parameterName)
        {
            if (_mixer.GetFloat(parameterName, out float db))
                return Mathf.Pow(10, db / 20);
            return 1f;
        }

        public void SetMasterVolume(float value) => SetVolume(_library.Settings.MasterVolumeParam, value);
        public void SetMusicVolume(float value) => SetVolume(_library.Settings.MusicVolumeParam, value);
        public void SetSfxVolume(float value) => SetVolume(_library.Settings.SfxVolumeParam, value);

        public float GetMasterVolume() => GetVolume(_library.Settings.MasterVolumeParam);
        public float GetMusicVolume() => GetVolume(_library.Settings.MusicVolumeParam);
        public float GetSfxVolume() => GetVolume(_library.Settings.SfxVolumeParam);

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
            (_activeMusicSource, _inactiveMusicSource) = (_inactiveMusicSource, _activeMusicSource);
            _fadeCoroutine = null;
        }
    }
}