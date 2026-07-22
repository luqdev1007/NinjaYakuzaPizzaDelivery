using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Per-instance точка вылета снаряда фонаря («дуло») со сцены. Снимается из
    /// LanternMuzzleAuthoring в GameplayBootstrap (по образцу PatrolRoute) и уезжает
    /// в фабрику. Направление тут НЕ хранится — оно считается снапшотом на героя
    /// в момент выстрела (LanternFireSystem). Фонарь стационарен, точка фиксируется
    /// один раз при спавне.
    /// </summary>
    public struct LanternMuzzleData
    {
        /// <summary>Мировая точка вылета снаряда (позиция «дула»).</summary>
        public Vector2 Origin;
    }
}
