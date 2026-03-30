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

        // Конструктор принимает миксер для управления приглушением при смерти
        public AudioService(AudioConfig config, AudioManager manager, AudioMixer mixer)
        {
            _config = config;
            _manager = manager;
            _mixer = mixer;
            _manager.OnMusicEnded += PlayNextFromPlaylist;
        }

        /// <summary>
        /// Проигрывает зацикленный звук. Использовать для Plunge, Slide и т.д.
        /// </summary>
        public void PlayLoopingSfx(string loopId, AudioCategoryType category, float volumeMultiplier = 1f)
        {
            if (_activeLoops.ContainsKey(loopId)) return;

            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            // Используем GetFreeSource напрямую из менеджера
            AudioSource source = _manager.PlaySfxReturnSource(data.Clip, data.Volume * volumeMultiplier, data.BasePitch);
            if (source != null)
            {
                source.loop = true;
                _activeLoops[loopId] = source;
            }
        }

        // В AudioService.cs
        public void PlaySfxVariation(string prefix, int minIndex, int maxIndex, float pitch)
        {
            int randomIndex = UnityEngine.Random.Range(minIndex, maxIndex + 1);
            var data = _config.GetById($"{prefix}{randomIndex}");

            if (data == null) return;

            // Проигрываем через менеджер с нашим высчитанным питчем
            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        public void StopLoopingSfx(string loopId)
        {
            if (_activeLoops.TryGetValue(loopId, out AudioSource source))
            {
                source.Stop();
                source.loop = false;
                _activeLoops.Remove(loopId);
            }
        }


        /// <summary>
        /// Проигрывает случайный звук из указанной категории (атака, шаги и т.д.)
        /// </summary>
        public void PlayRandomSfx(AudioCategoryType category, bool useRandomPitch = true, float multiplier = 1)
        {
            if (IsSpamming(category.ToString())) return;

            var data = _config.GetRandomFromCategory(category);
            if (data == null) return;

            _lastPlayedTimes[category.ToString()] = Time.time;
            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch * multiplier);
        }

        /// <summary>
        /// Проигрывает звук по префиксу (например, "BatHit" или "ImpDeath")
        /// </summary>
        public void PlaySfxByPrefix(string prefix, bool useRandomPitch = true)
        {
            Debug.Log($"[AudioService] Searching for prefix: {prefix}"); // Добавь это

            if (IsSpamming(prefix)) return;

            var data = _config.GetRandomByPrefix(prefix);
            if (data == null) return;

            _lastPlayedTimes[prefix] = Time.time;
            float pitch = useRandomPitch ? data.BasePitch * Random.Range(0.9f, 1.1f) : data.BasePitch;
            _manager.PlaySfx(data.Clip, data.Volume, pitch);
        }

        /// <summary>
        /// Запускает плейлист по ID (Menu, Gameplay) с логикой псевдорандома
        /// </summary>
        public void StartPlaylist(string playlistId)
        {
            var playlist = _config.GetPlaylist(playlistId);
            if (playlist == null) return;

            // Автоматически возвращаем громкость в норму при смене плейлиста
            SetMusicMuted(false);

            _currentPlaylist = playlist;
            _lastTrackIndex = -1;
            PlayNextFromPlaylist();
        }

        /// <summary>
        /// Управляет приглушением музыки (например, при поражении) через Mixer
        /// </summary>
        public void SetMusicMuted(bool isMuted)
        {
            // -15f или -20f создают приятный эффект "фоновости", -80f — полная тишина
            float targetVolume = isMuted ? -15f : 0f;
            _mixer.SetFloat("MusicVolume", targetVolume);
        }

        private void PlayNextFromPlaylist()
        {
            if (_currentPlaylist == null || _currentPlaylist.Tracks.Count == 0) return;

            int nextIndex;
            if (_currentPlaylist.Tracks.Count == 1)
            {
                nextIndex = 0;
            }
            else
            {
                // Выбираем следующий трек так, чтобы он не повторял предыдущий
                do { nextIndex = Random.Range(0, _currentPlaylist.Tracks.Count); }
                while (nextIndex == _lastTrackIndex);
            }

            _lastTrackIndex = nextIndex;
            // loop: false нужен, чтобы AudioManager вызвал OnMusicEnded по завершении клипа
            _manager.PlayMusic(_currentPlaylist.Tracks[nextIndex], _currentPlaylist.Volume, false);
        }

        private bool IsSpamming(string id)
        {
            if (_lastPlayedTimes.TryGetValue(id, out float lastTime))
                return (Time.time - lastTime) < GlobalSfxCooldown;
            return false;
        }

        // В AudioService.cs добавь корутину для миксера:
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

        // Добавьте этот метод в AudioService.cs
        public string PlaySfxVariationLoop(string prefix, int minIndex, int maxIndex, float volumeMultiplier = 1f)
        {
            int randomIndex = UnityEngine.Random.Range(minIndex, maxIndex + 1);
            string fullId = $"{prefix}{randomIndex}";
            var data = _config.GetById(fullId);

            if (data == null) return null;

            // Проверяем, не запущен ли уже этот конкретный цикл
            if (_activeLoops.ContainsKey(fullId)) return fullId;

            AudioSource source = _manager.PlaySfxReturnSource(data.Clip, data.Volume * volumeMultiplier, data.BasePitch);
            if (source != null)
            {
                source.loop = true;
                _activeLoops[fullId] = source;
                return fullId; // Возвращаем ID, чтобы потом вызвать StopLoopingSfx
            }

            return null;
        }

        // Переименуем старый метод для консистентности, либо просто добавим обертку:
        public void StopSfx(string loopId) => StopLoopingSfx(loopId);

        // Измени SetMusicMuted, чтобы он мог быть мгновенным или плавным
        public void SetMusicMuted(bool isMuted, float duration = 0f, ICoroutinesPerformer performer = null)
        {
            float targetVolume = isMuted ? -15f : 0f;

            if (duration > 0 && performer != null)
            {
                performer.StartPerform(FadeMixerGroup("MusicVolume", targetVolume, duration));
            }
            else
            {
                _mixer.SetFloat("MusicVolume", targetVolume);
            }
        }

        public void SetPitch(string loopId, float targetPitch)
        {
            // Проверяем, есть ли такой запущенный цикл в словаре
            if (_activeLoops.TryGetValue(loopId, out AudioSource source))
            {
                // Если источник всё еще проигрывается, меняем питч
                if (source != null && source.isPlaying)
                {
                    source.pitch = targetPitch;
                }
            }
        }
    }
}