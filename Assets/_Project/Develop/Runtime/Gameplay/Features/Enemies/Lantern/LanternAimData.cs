using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Per-instance прицел фонаря со сцены: откуда и куда он стреляет. Снимается
    /// из LanternAimAuthoring в GameplayBootstrap (по образцу PatrolRoute) и
    /// уезжает в фабрику. Фонарь стационарен, поэтому обе величины фиксируются
    /// один раз при спавне и в рантайме не пересчитываются.
    /// </summary>
    public struct LanternAimData
    {
        /// <summary>Мировая точка вылета снаряда (позиция «дула»).</summary>
        public Vector2 Origin;

        /// <summary>Направление выстрела (мировое, нормализуется в фабрике).</summary>
        public Vector2 Direction;
    }
}
