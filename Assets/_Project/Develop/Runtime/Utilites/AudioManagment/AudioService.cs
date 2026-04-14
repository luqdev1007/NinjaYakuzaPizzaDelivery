using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    public class AudioService
    {
        private readonly AudioConfig _config;
        private readonly AudioManager _manager;
        private readonly AudioMixer _mixer;

        private readonly Dictionary<string, float> _lastPlayedTimes = new Dictionary<string, float>();
        private readonly Dictionary<string, AudioSource> _activeLoops = new Dictionary<string, AudioSource>();

        private MusicPlaylist _currentPlaylist;
        private int _lastTrackIndex = -1;
        private const float GlobalSfxCooldown = 0.04f;

        public AudioService(AudioConfig config, AudioManager manager, AudioMixer mixer)
        {
            _config = config;
            _manager = manager;
            _mixer = mixer;
            _manager.OnMusicEnded += PlayNextFromPlaylist;
        }

        public float GetMasterVolume() => GetVolume("MasterVolume");
        public float GetMusicVolume() => GetVolume("MusicVolume");
        public float GetSFXVolume() => GetVolume("SFXVolume");

        private float GetVolume(string parameter)
        {
            if (_mixer.GetFloat(parameter, out float db))
            {
                return DbToLinear(db);
            }
            return 1f;
        }


        private float LinearToDb(float linear) => linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
        private float DbToLinear(float db) => Mathf.Pow(10f, db / 20f);

        // --- ГРОМКОСТЬ ---
        // public void SetMusicMuted(bool isMuted) => _mixer.SetFloat("MusicVolume", isMuted ? -80f : 0f);

        public void SetVolume(string parameter, float linear) => _mixer.SetFloat(parameter, LinearToDb(linear));

        public void SetMasterVolume(float v) => SetVolume("MasterVolume", v);
        public void SetMusicVolume(float v) => SetVolume("MusicVolume", v);
        public void SetSFXVolume(float v) => SetVolume("SFXVolume", v);
        public void SetUIVolume(float v) => SetVolume("UIVolume", v);

        // --- SFX ЛОГИКА ---

        // ТОТ САМЫЙ МЕТОД ДЛЯ ПРОВЕРКИ ВАРИАЦИЙ
        public int GetVariationCount(string prefix) => _config.GetVariationCount(prefix);

        public void PlaySfxByPrefixAuto(string prefix, float pitch = 1f)
        {
            int count = _config.GetVariationCount(prefix);
            if (count > 0) PlaySfxDirect($"{prefix}{UnityEngine.Random.Range(1, count + 1)}", pitch);
            else PlaySfxDirect(prefix, pitch);
        }

        public void PlaySfxVariation(string prefix, int min, int max, float pitch = 1f)
        {
            string id = $"{prefix}{UnityEngine.Random.Range(min, max + 1)}";
            PlaySfxDirect(id, pitch);
        }

        public void PlaySfxDirect(string id, float pitch = 1f)
        {
            if (IsSpamming(id)) return;
            var data = _config.GetById(id);
            if (data != null)
            {
                _lastPlayedTimes[id] = Time.time;
                _manager.PlaySfx(data.Clip, data.Volume, pitch);
            }
        }

        public void PlayRandomSfx(AudioCategoryType category, bool useRandomPitch = true, float multiplier = 1)
        {
            if (IsSpamming(category.ToString())) return;
            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            _lastPlayedTimes[category.ToString()] = Time.time;
            float pitch = useRandomPitch ? data.BasePitch * UnityEngine.Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch * multiplier, category == AudioCategoryType.UI);
        }

        // --- LOOP ---
        public string PlaySfxVariationLoop(string prefix, int min, int max, float volMult = 1f)
        {
            string id = $"{prefix}{UnityEngine.Random.Range(min, max + 1)}";
            if (_activeLoops.ContainsKey(id)) return id;

            var data = _config.GetById(id);
            if (data == null) return null;

            AudioSource source = _manager.PlaySfxReturnSource(data.Clip, data.Volume * volMult, data.BasePitch);
            if (source != null) { source.loop = true; _activeLoops[id] = source; return id; }
            return null;
        }

        public void SetPitch(string loopId, float targetPitch)
        {
            if (!string.IsNullOrEmpty(loopId) && _activeLoops.TryGetValue(loopId, out var source))
                if (source != null) source.pitch = targetPitch;
        }

        public void StopSfx(string loopId)
        {
            if (!string.IsNullOrEmpty(loopId) && _activeLoops.TryGetValue(loopId, out var s))
            {
                if (s != null) { s.Stop(); s.loop = false; }
                _activeLoops.Remove(loopId);
            }
        }

        // --- МУЗЫКА ---
        public void StartPlaylist(string id)
        {
            _currentPlaylist = _config.GetPlaylist(id);
            if (_currentPlaylist != null) { _lastTrackIndex = -1; PlayNextFromPlaylist(); }
        }

        private void PlayNextFromPlaylist()
        {
            if (_currentPlaylist == null || _currentPlaylist.Tracks.Count == 0) return;
            int idx = _currentPlaylist.Tracks.Count == 1 ? 0 : UnityEngine.Random.Range(0, _currentPlaylist.Tracks.Count);
            if (idx == _lastTrackIndex && _currentPlaylist.Tracks.Count > 1) idx = (idx + 1) % _currentPlaylist.Tracks.Count;

            _lastTrackIndex = idx;
            _manager.PlayMusic(_currentPlaylist.Tracks[idx], _currentPlaylist.Volume, false);
        }

        private bool IsSpamming(string id) => _lastPlayedTimes.TryGetValue(id, out float t) && (Time.time - t) < GlobalSfxCooldown;
    }
}