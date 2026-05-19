using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class IntentJump : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IntentDash : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
