using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Effects
{
    public class SleepTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
