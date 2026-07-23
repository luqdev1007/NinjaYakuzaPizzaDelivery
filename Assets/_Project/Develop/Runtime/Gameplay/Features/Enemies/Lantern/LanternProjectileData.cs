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
        /// ContactsDetectingMask ОДИН, без геометрии: буфер контактов нужен только
        /// для урона. Деспавн об стену ведётся отдельным кастом по BlockMask
        /// (см. LanternProjectileSystem), а НЕ через контактный буфер.
        /// </summary>
        public LayerMask TargetMask;

        /// <summary>
        /// Геометрия, об которую снаряд гаснет: Ground + Wall.
        ///
        /// НЕ КОПИЯ SightBlockMask СЛАЙМА. У слайма в маске есть Default(0), а на
        /// Default лежит LevelBounds — охватывающий триггер на весь уровень. Именно
        /// он в прошлой итерации убивал снаряд на первом же кадре. Смысл этого поля
        /// отдельный от слаймового, поэтому и имя другое: подставишь сюда слаймовую
        /// маску — вернёшь ту же поломку.
        ///
        /// Enemies(9) сюда НЕ входит: на нём лежат и сам фонарь, и тело снаряда.
        /// </summary>
        public LayerMask BlockMask;

        /// <summary>Путь к префабу снаряда в Resources.</summary>
        public string PrefabPath;
    }
}
