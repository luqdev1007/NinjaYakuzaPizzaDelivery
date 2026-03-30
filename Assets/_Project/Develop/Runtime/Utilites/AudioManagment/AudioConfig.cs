using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField] private List<AudioCategory> _categories;
        [SerializeField] private List<MusicPlaylist> _playlists;

        public AudioData GetRandomFromCategory(AudioCategoryType type)
        {
            var category = _categories.FirstOrDefault(c => c.Type == type);
            if (category == null || category.Clips == null || category.Clips.Count == 0) return null;
            return category.Clips[UnityEngine.Random.Range(0, category.Clips.Count)];
        }

        public AudioData GetRandomByPrefix(string prefix)
        {
            var matches = _categories
                .SelectMany(c => c.Clips)
                .Where(d => d.Id.StartsWith(prefix))
                .ToList();

            return matches.Count == 0 ? null : matches[UnityEngine.Random.Range(0, matches.Count)];
        }

        public AudioData GetById(string id)
        {
            return _categories.SelectMany(c => c.Clips).FirstOrDefault(d => d.Id == id);
        }

        public MusicPlaylist GetPlaylist(string id) => _playlists.FirstOrDefault(p => p.Id == id);
    }

    [Serializable]
    public class AudioCategory
    {
        public AudioCategoryType Type;
        public List<AudioData> Clips;
    }

    [Serializable]
    public class AudioData
    {
        public string Id;
        public AudioClip Clip;
        [Range(0, 1)] public float Volume = 1f;
        [Range(0.1f, 3f)] public float BasePitch = 1f;
    }

    [Serializable]
    public class MusicPlaylist
    {
        public string Id;
        public List<AudioClip> Tracks;
        [Range(0, 1)] public float Volume = 0.5f;
    }

    public enum AudioCategoryType
    {
        HeroAttackSwing,
        HeroAttackHit,
        Footsteps,
        UI,
        Music,
        TakeDamage,
        Death
    }
}