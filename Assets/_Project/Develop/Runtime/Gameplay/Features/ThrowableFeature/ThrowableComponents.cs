using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ThrowEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class ThrowRequest : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }
}
