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
        private const float GlobalSfxCooldown = 0.05f;

        public AudioService(AudioConfig config, AudioManager manager, AudioMixer mixer)
        {
            _config = config;
            _manager = manager;
            _mixer = mixer;
            _manager.OnMusicEnded += PlayNextFromPlaylist;
        }

        // --- МГНОВЕННЫЕ SFX (УДАРЫ, РЫВКИ) ---

        /// <summary>
        /// Автоматически находит количество вариаций в конфиге и играет рандомную.
        /// Например, при префиксе "EnemyHit" найдет EnemyHit1, EnemyHit2 и т.д.
        /// </summary>
        public void PlaySfxByPrefixAuto(string prefix, float pitch)
        {
            int count = _config.GetVariationCount(prefix);

            if (count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(1, count + 1);
                PlaySfxDirect($"{prefix}{randomIndex}", pitch);
            }
            else
            {
                // Если вариаций с цифрами не найдено, пробуем проиграть как одиночный ID
                PlaySfxDirect(prefix, pitch);
            }
        }

        /// <summary>
        /// Проигрывает вариацию в заданном диапазоне (для обратной совместимости).
        /// </summary>
        public void PlaySfxVariation(string prefix, int minIndex, int maxIndex, float pitch)
        {
            int randomIndex = UnityEngine.Random.Range(minIndex, maxIndex + 1);
            PlaySfxDirect($"{prefix}{randomIndex}", pitch);
        }

        private void PlaySfxDirect(string id, float pitch)
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
            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch * multiplier);
        }

        // --- ЗАЦИКЛЕННЫЕ SFX (ГЛАЙД, СКОЛЬЖЕНИЕ ПО СТЕНЕ) ---

        /// <summary>
        /// Запускает зацикленный звук из вариаций. Возвращает ID для остановки или смены питча.
        /// </summary>
        public string PlaySfxVariationLoop(string prefix, int minIndex, int maxIndex, float volumeMultiplier = 1f)
        {
            int randomIndex = UnityEngine.Random.Range(minIndex, maxIndex + 1);
            string fullId = $"{prefix}{randomIndex}";

            if (_activeLoops.ContainsKey(fullId)) return fullId;

            var data = _config.GetById(fullId);
            if (data == null) return null;

            AudioSource source = _manager.PlaySfxReturnSource(data.Clip, data.Volume * volumeMultiplier, data.BasePitch);
            if (source != null)
            {
                source.loop = true;
                _activeLoops[fullId] = source;
                return fullId;
            }
            return null;
        }

        /// <summary>
        /// Динамически меняет питч у активного зацикленного звука (например, от скорости падения).
        /// </summary>
        public void SetPitch(string loopId, float targetPitch)
        {
            if (string.IsNullOrEmpty(loopId)) return;
            if (_activeLoops.TryGetValue(loopId, out AudioSource source) && source != null)
            {
                source.pitch = targetPitch;
            }
        }

        /// <summary>
        /// Останавливает зацикленный звук по его ID.
        /// </summary>
        public void StopSfx(string loopId)
        {
            if (string.IsNullOrEmpty(loopId)) return;
            if (_activeLoops.TryGetValue(loopId, out AudioSource source))
            {
                if (source != null)
                {
                    source.Stop();
                    source.loop = false;
                }
                _activeLoops.Remove(loopId);
            }
        }

        // --- МУЗЫКА И ПЛЕЙЛИСТЫ ---

        public void StartPlaylist(string playlistId)
        {
            var playlist = _config.GetPlaylist(playlistId);
            if (playlist == null) return;

            SetMusicMuted(false);
            _currentPlaylist = playlist;
            _lastTrackIndex = -1;
            PlayNextFromPlaylist();
        }

        private void PlayNextFromPlaylist()
        {
            if (_currentPlaylist == null || _currentPlaylist.Tracks.Count == 0) return;

            int nextIndex;
            if (_currentPlaylist.Tracks.Count == 1) nextIndex = 0;
            else
            {
                do { nextIndex = Random.Range(0, _currentPlaylist.Tracks.Count); }
                while (nextIndex == _lastTrackIndex);
            }

            _lastTrackIndex = nextIndex;
            _manager.PlayMusic(_currentPlaylist.Tracks[nextIndex], _currentPlaylist.Volume, false);
        }

        // --- УПРАВЛЕНИЕ МИКСЕРОМ ---

        public void SetMusicMuted(bool isMuted, float duration = 0f, ICoroutinesPerformer performer = null)
        {
            float targetVolume = isMuted ? -15f : 0f;

            if (duration > 0 && performer != null)
                performer.StartPerform(FadeMixerGroup("MusicVolume", targetVolume, duration));
            else
                _mixer.SetFloat("MusicVolume", targetVolume);
        }

        public IEnumerator FadeMixerGroup(string parameterName, float targetDb, float duration)
        {
            _mixer.GetFloat(parameterName, out float startValue);
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float newValue = Mathf.Lerp(startValue, targetDb, timer / duration);
                _mixer.SetFloat(parameterName, newValue);
                yield return null;
            }
            _mixer.SetFloat(parameterName, targetDb);
        }

        private bool IsSpamming(string id)
        {
            if (_lastPlayedTimes.TryGetValue(id, out float lastTime))
                return (Time.time - lastTime) < GlobalSfxCooldown;
            return false;
        }
    }
}