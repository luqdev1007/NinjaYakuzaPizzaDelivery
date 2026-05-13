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

        public void Play(SoundData data, Action<AudioEmitter> onComplete)
        {
            _onComplete = onComplete;

            Source.clip = data.Clips[UnityEngine.Random.Range(0, data.Clips.Length)];
            Source.volume = data.Volume;
            Source.pitch = UnityEngine.Random.Range(data.PitchMin, data.PitchMax);
            Source.outputAudioMixerGroup = data.Group;
            Source.loop = false;

            Source.Play();

            Invoke(nameof(ReturnToPool), Source.clip.length + 0.1f);
        }

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

            Source.clip = data.Clips[UnityEngine.Random.Range(0, data.Clips.Length)];
            Source.volume = data.Volume;
            Source.pitch = UnityEngine.Random.Range(data.PitchMin, data.PitchMax);
            Source.outputAudioMixerGroup = data.Group;

            Source.Play();
            Invoke(nameof(ReturnToPool), Source.clip.length + 0.1f);
        }

        private void ReturnToPool() => _onComplete?.Invoke(this);
    }
}