using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public abstract class BuffConfig : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public float Duration { get; private set; } = 6f;

        [Header("Pickup Flight")]
        [field: SerializeField] public float ArcHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float TravelTime { get; private set; } = 0.5f;

        public abstract IBuffEffect CreateEffect();
    }
}