using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/MetaLoot", fileName = "NewMetaLoot")]
    public class MetaLootConfig : LootConfig
    {
        // Здесь могут быть данные, которые мы узнаем только в конце уровня
        [field: SerializeField] public string SecretId { get; private set; }
    }
}