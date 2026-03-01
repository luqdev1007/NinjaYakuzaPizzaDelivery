using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles
{
    public class MoveSinkSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class RotationSinkSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
