using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Style
{
    [CreateAssetMenu(fileName = "StyleActionsConfig", menuName = "Configs/Gameplay/Style/Actions Config")]
    public class StyleActionsConfig : ScriptableObject
    {
        [Header("Combat — очки за попадание (флэт, не зависит от величины урона)")]
        [Tooltip("Обычная атака катаной, попадание по цели (враг или проп)")]
        public float LightAttackPoints = 8f;

        [Tooltip("Урон телом на скорости — дэш/пике сквозь цель. ПОКА НЕ ПОДКЛЮЧЕНО: нет сигнала из LethalContactMovementSystem, см. чат.")]
        public float SpeedDamagePoints = 15f;

        [Header("Movement — очки за факт исполнения техники (не за удержание состояния)")]
        [Tooltip("Инициация рывка (дэш)")]
        public float DashPoints = 8f;

        [Tooltip("Отскок от стены")]
        public float WallJumpPoints = 10f;

        [Tooltip("Момент зацепа катаной за стену — НЕ за каждый кадр висения на ней")]
        public float WallHangAttachPoints = 10f;

        [Tooltip("Момент успешного зацепа крюком-кошкой — НЕ за каждый кадр полёта по инерции")]
        public float GrappleAttachPoints = 12f;

        [Tooltip("Срабатывание оглушающего AoE при приземлении после смертельного пике")]
        public float PlungeSlamPoints = 25f;

        [Header("Kill / Destroy")]
        [Tooltip("Бонус сверху, если этим ударом цель была уничтожена. ПОКА НЕ ПРИМЕНЯЕТСЯ для LightAttack — см. чат.")]
        public float KillBonus = 15f;

        [Header("Diversity chain")]
        [Tooltip("Множитель очков, если тип действия не встречался в последних N действиях (полностью свежий тип)")]
        public float DiversityMultiplier = 1.5f;

        [Tooltip("Сколько последних действий хранится в истории для проверки 'свежести' (бонус 1.5x)")]
        public int DiversityHistorySize = 3;

        [Header("Repeat cooldown — защита от спама у стены")]
        [Tooltip("Сколько реальных секунд тип действия должен 'остывать' после использования, прежде чем снова дать полную/бонусную цену. Спам внутри окна штрафуется MinDiversityMultiplier независимо от чередования с другими типами.")]
        public float RepeatCooldownSeconds = 5f;

        [Tooltip("Множитель очков для типа действия, который ещё не остыл. 0 = совсем без очков (как раньше для прямого повтора), >0 = небольшой утешительный доход.")]
        public float MinDiversityMultiplier = 0.1f;

        [Header("Penalty")]
        [Tooltip("Сколько под-рангов теряет игрок при получении урона")]
        public int RanksToDropOnDamage = 1;
    }
}