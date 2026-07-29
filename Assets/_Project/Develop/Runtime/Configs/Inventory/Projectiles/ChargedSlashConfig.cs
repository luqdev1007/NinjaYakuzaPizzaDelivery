using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    /// <summary>
    /// Уставки заряженного слэша. Раньше жили литералами прямо в теле
    /// ProjectileFactory.CreateChargedSlashProjectile под комментарием
    /// "// settings (config)" — соседний CreateSlimeTongue в докблоке прямо
    /// назвал эту практику ненаследуемой.
    ///
    /// Вынос — пререквизит апгрейдов, а не косметика: апгрейд считается как
    /// «база + бонус», и пока база была магическим литералом внутри метода,
    /// её нельзя было ни забалансить, ни прочитать снаружи.
    ///
    /// Отдельный ScriptableObject, а не поле в MainHeroConfig: слэш — снаряд,
    /// и его уставки принадлежат снаряду, как ShurikenConfig принадлежит
    /// сюрикену. В MainHeroConfig.Attack остаётся только то, что относится к
    /// герою (SlashAttackChargeRequiredTime — время удержания).
    ///
    /// НЕ наследует ThrowableItemConfig: слэш не предмет инвентаря, у него нет
    /// ни зарядов в сумке, ни слота, ни Id расходника, и попадание в иерархию
    /// InventoryItemConfig затащило бы его в ротацию колёсиком.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ChargedSlashConfig",
        menuName = "Configs/Gameplay/Projectiles/Charged Slash")]
    public class ChargedSlashConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
            = "Entities/Projectiles/ChargedSlashProjectile";

        [field: SerializeField, Min(0f)] public float Damage { get; private set; } = 10f;

        [field: SerializeField, Min(0f)] public float Speed { get; private set; } = 15f;

        [field: SerializeField, Min(0f), Tooltip("Секунды жизни снаряда. Пробег = Speed * LifeTime")]
        public float LifeTime { get; private set; } = 2f;

        [field: SerializeField] public LayerMask HitMask { get; private set; }

        /// <summary>
        /// Множитель к размеру хитбокса снаряда. База 1 = размер BoxCollider2D
        /// как он лежит в префабе; ветка «дальность+радиус» домножает поверх.
        ///
        /// Множитель, а не абсолютный размер: сам размер задан в префабе вместе
        /// с визуалом, и держать его вторым числом в конфиге значило бы завести
        /// второго хозяина одной величины.
        /// </summary>
        [field: SerializeField, Min(0.01f)] public float HitboxScale { get; private set; } = 1f;
    }
}
