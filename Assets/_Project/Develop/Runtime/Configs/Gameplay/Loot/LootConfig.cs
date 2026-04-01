using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    public abstract class LootConfig : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; } // Ссылка на префаб напрямую удобнее для малых проектов

        [Header("Drop Settings")]
        [field: SerializeField, Range(0, 100)] public float DropChance { get; private set; } = 100f;
        [field: SerializeField] public Vector2 MinMaxLaunchForce { get; private set; } = new Vector2(3f, 7f);

        [Header("Magnet Settings")]
        [field: SerializeField] public float MagnetRadius { get; private set; } = 5f;
        [field: SerializeField] public float PullSpeed { get; private set; } = 10f;
    }
}