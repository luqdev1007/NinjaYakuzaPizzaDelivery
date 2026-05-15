using Assets._Project.Develop.Runtime.Configs.Audio;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.AudioManagment
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour
    {
        public AudioSource Source { get; private set; }
        private Action<AudioEmitter> _onComplete;

        private void Awake() => Source = GetComponent<AudioSource>();

        public void Play(SoundData data, Vector3? position, Action<AudioEmitter> onComplete)
        {
            _onComplete = onComplete;

            if (position.HasValue)
            {
                transform.position = position.Value;
                Source.spatialBlend = 1f;
            }
            else
            {
                Source.spatialBlend = 0f;
            }

            if (data.Clips != null && data.Clips.Length > 0)
            {
                Source.clip = data.Clips[UnityEngine.Random.Range(0, data.Clips.Length)];
            }
            else
            {
                Debug.LogWarning($"SoundData with key {data.Key} has no clips!");
                ReturnToPool();
                return;
            }

            Source.volume = data.Volume;
            Source.pitch = UnityEngine.Random.Range(data.PitchMin, data.PitchMax);
            Source.outputAudioMixerGroup = data.Group;
            Source.loop = false;

            Source.Play();

            float duration = Source.clip.length / Mathf.Max(0.01f, Source.pitch);
            Invoke(nameof(ReturnToPool), duration + 0.1f);
        }

        private void ReturnToPool() => _onComplete?.Invoke(this);

        private void OnDisable() => CancelInvoke();
    }
}