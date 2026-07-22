using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Уставки одного снаряда фонаря. Едут в фабрику параметром, а не хардкодятся
    /// в её теле: тот же урок, что зафиксирован в ProjectileFactory.CreateSlimeTongue
    /// (практику "// settings (config)" из соседних методов не наследуем). Значения
    /// приезжают из LanternConfig, их прокидывает LanternFireSystem в момент выстрела.
    /// </summary>
    public struct LanternProjectileData
    {
        /// <summary>Скорость полёта, ед/с.</summary>
        public float Speed;

        /// <summary>Время жизни, сек. По истечении снаряд деспавнится сам.</summary>
        public float LifeTime;

        /// <summary>Контактный урон герою телом снаряда.</summary>
        public float ContactDamage;

        /// <summary>
        /// Слой цели контактного урона — герой (Characters). Складывается с
        /// SightBlockMask в ContactsDetectingMask, чтобы буфер контактов ловил и
        /// героя (для урона), и стены (для деспавна).
        /// </summary>
        public LayerMask TargetMask;

        /// <summary>
        /// Геометрия, об которую снаряд гаснет. Уезжает в DeathMask — тот же
        /// паттерн деспавна, что у сюрикена (DeathMaskTouchDetectorSystem).
        /// </summary>
        public LayerMask SightBlockMask;

        /// <summary>Путь к префабу снаряда в Resources.</summary>
        public string PrefabPath;
    }
}
