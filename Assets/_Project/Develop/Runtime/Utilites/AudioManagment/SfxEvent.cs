using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.AudioManagement
{
    [CreateAssetMenu(fileName = "NewSfxEvent", menuName = "Configs/Audio/Sfx Event")]
    public class SfxEvent : ScriptableObject
    {
        public AudioClip[] Clips;

        [Range(0f, 1f)] 
        public float Volume = 1f;

        [Tooltip("X - минимальный питч, Y - максимальный")]
        public Vector2 PitchRange = new Vector2(0.9f, 1.1f);

        [Tooltip("Защита от спама звуком")]
        public float Cooldown = 0.05f;

        public bool IsUi = false;

        public AudioClip GetRandomClip()
        {
            if (Clips == null || Clips.Length == 0) 
                return null;

            return Clips[Random.Range(0, Clips.Length)];
        }

        public float GetRandomPitch() => Random.Range(PitchRange.x, PitchRange.y);
    }
}