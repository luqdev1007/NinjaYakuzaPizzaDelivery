using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Audio
{
    [CreateAssetMenu(fileName = "PlaylistData", menuName = "Configs/Audio/PlaylistData")]
    public class PlaylistData : ScriptableObject
    {
        public string Key;
        public List<MusicDataEntry> Tracks;
        public bool Shuffle = true;
    }
}