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
        /// Слой цели контактного урона — герой (Characters). Идёт в
        /// ContactsDetectingMask: буфер контактов нужен только для урона, снаряд
        /// летит сквозь стены, поэтому геометрия в маску не входит.
        /// </summary>
        public LayerMask TargetMask;

        /// <summary>Путь к префабу снаряда в Resources.</summary>
        public string PrefabPath;
    }
}
