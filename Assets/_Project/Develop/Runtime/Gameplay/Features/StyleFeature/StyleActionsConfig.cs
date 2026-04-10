using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Style
{
    [CreateAssetMenu(fileName = "StyleActionsConfig", menuName = "Configs/Gameplay/Style/Actions Config")]
    public class StyleActionsConfig : ScriptableObject
    {
        [Header("Combat")]
        public float KillBasePoints = 100f;
        public float DamagePointMultiplier = 0.5f; // Очки за нанесенный урон
        public float FreshnessBonus = 1.5f;        // Множитель за новый тип урона

        [Header("Movement")]
        public float UpwardAccelerationPoints = 10f; // Очков в секунду при взлете
        public float DashPoints = 15f;

        [Header("Collectibles")]
        public float CoinCollectPoints = 5f;
        public float MemoryFragmentPoints = 25f;

        [Header("Penalties")]
        public int RanksToDropOnDamage = 2; // Сколько рангов скидываем при получении урона
    }
}