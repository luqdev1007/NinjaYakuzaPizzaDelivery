using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    public abstract class LootConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public LootTypes LootType { get; private set; }

        [Header("Drop Settings")]
        [field: SerializeField] public Vector2 LaunchForceRangeX { get; private set; } = new Vector2(-7f, 7f);
        [field: SerializeField] public Vector2 LaunchForceRangeY { get; private set; } = new Vector2(6f, 10f);
        [field: SerializeField] public Vector2 GravityRange { get; private set; } = new Vector2(4f, 6f);

        [Header("Life Cycle Settings")]
        [field: SerializeField] public float SpawnDuration { get; private set; } = 1f; 
        [field: SerializeField] public float LifeTime { get; private set; } = 5f;

        [Header("Collect Settings")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 12f;
        [field: SerializeField] public float ArcHeight { get; private set; } = 2.5f;
        [field: SerializeField] public float TravelTime { get; private set; } = 1.0f;
    }
}
