using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Style
{
    [CreateAssetMenu(fileName = "StyleActionsConfig", menuName = "Configs/Gameplay/Style/Actions Config")]
    public class StyleActionsConfig : ScriptableObject
    {
        [Header("Combat")]
        [Tooltip("Базовые очки за убийство врага")]
        public float KillBasePoints = 100f;

        [Tooltip("Множитель очков за каждую единицу нанесенного урона")]
        public float DamagePointMultiplier = 0.5f;

        [Tooltip("Бонусный множитель за использование нового типа атаки (стиль свежести)")]
        public float FreshnessBonus = 1.5f;

        [Header("Movement")]
        [Tooltip("Количество очков в секунду, начисляемых при быстром взлете вверх")]
        public float UpwardAccelerationPoints = 10f;

        [Tooltip("Фиксированные очки за выполнение рывка (Dash)")]
        public float DashPoints = 15f;

        [Header("Collectibles")]
        [Tooltip("Очки за подбор одной монеты")]
        public float CoinCollectPoints = 5f;

        [Tooltip("Очки за подбор фрагмента памяти")]
        public float MemoryFragmentPoints = 25f;

        [Header("Penalties")]
        [Tooltip("Количество рангов, которые теряет игрок при получении урона")]
        public int RanksToDropOnDamage = 2;

        [field: SerializeField] public float LootPickupPoints { get; private set; } = 50f; 
    }
}
