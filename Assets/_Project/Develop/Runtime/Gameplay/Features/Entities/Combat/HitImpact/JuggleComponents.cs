using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.HitImpact
{
    public class AerialHangForce : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }
}
