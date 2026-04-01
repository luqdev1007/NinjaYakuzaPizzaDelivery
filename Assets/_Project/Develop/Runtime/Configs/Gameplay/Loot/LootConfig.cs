using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    public abstract class LootConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }

        [Header("Audio Settings")]
        [field: SerializeField] public string CollectSoundId { get; private set; } = "CoinCollect";

        [Header("Drop & Physics (Взрыв при спавне)")]
        [field: SerializeField] public Vector2 LaunchForceX { get; private set; } = new Vector2(-7f, 7f);
        [field: SerializeField] public Vector2 LaunchForceY { get; private set; } = new Vector2(6f, 10f);
        [field: SerializeField] public Vector2 GravityRange { get; private set; } = new Vector2(4f, 6f);

        [Header("Life Time (Таймеры)")]
        [field: SerializeField] public float SpawnDuration { get; private set; } = 1f; // Время "разлета"
        [field: SerializeField] public float LifeTime { get; private set; } = 5f;      // Сколько лежит на земле

        [Header("Magnet & Movement (Полет к игроку)")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 12f;
        [field: SerializeField] public float ArcHeight { get; private set; } = 2.5f; // Высота прыжка при полете
        [field: SerializeField] public float TravelTime { get; private set; } = 1.0f;  // Время полета до героя
    }
}