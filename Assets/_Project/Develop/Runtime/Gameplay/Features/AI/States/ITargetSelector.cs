using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public interface ITargetSelector
    {
        // Legacy-перегрузка: удаляется на этапе маршрутизации через единый селектор.
        Entity SelectTargetFrom(IEnumerable<Entity> targets);

        Entity SelectTargetFrom(IEnumerable<Entity> targets, Entity excluded, float sqrRadius);
    }
}
