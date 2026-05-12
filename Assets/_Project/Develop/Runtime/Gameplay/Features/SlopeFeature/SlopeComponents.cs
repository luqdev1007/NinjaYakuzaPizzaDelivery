using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class IsOnSlope : IEntityComponent 
    { 
        public ReactiveVariable<bool> Value = new(); 
    }

    public class SlopeMask : IEntityComponent
    { 
        public LayerMask Value; 
    }

    public class SlopeMaxAccumSpeed : IEntityComponent
    { 
        public ReactiveVariable<float> Value; 
    }

    public class SlopeAccumGainRate : IEntityComponent 
    { 
        public ReactiveVariable<float> Value;
    }

    public class SlopeAccumSpeed : IEntityComponent 
    {
        public ReactiveVariable<float> Value = new(0f); 
    }

    public class SlopeMinAngle : IEntityComponent
    { 
        public ReactiveVariable<float> Value;
    }

    public class SlopeMaxAngle : IEntityComponent
    { 
        public ReactiveVariable<float> Value; 
    }
}