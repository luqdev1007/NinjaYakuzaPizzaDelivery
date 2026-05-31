using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot; // Не забываем юзинг таблицы лута
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Props
{
    [CreateAssetMenu(fileName = "NewPropConfig", menuName = "Configs/Props/PropConfig")]
    public class PropConfig : ScriptableObject
    {
        [Header("Life Cycle")]
        [SerializeField] private float _maxHealth = 10f;

        [Header("Loot")]
        [SerializeField] private LootTableConfig _lootTable; // Ссылка на таблицу лута

        public float MaxHealth => _maxHealth;
        public LootTableConfig LootTable => _lootTable; // Геттер
    }
}