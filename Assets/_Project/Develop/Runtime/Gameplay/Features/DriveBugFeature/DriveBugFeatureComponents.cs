using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature
{
    public class DriveAvailableJumps : IEntityComponent
    {
        public ReactiveVariable<int> Value; // Счетчик бонусных прыжков
    }

    public class IsDriveActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class DriveDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value; // Длительность свободного полета
    }

    public class DriveGravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value; // Насколько легким становится герой (например, 0 или 0.1)
    }
}