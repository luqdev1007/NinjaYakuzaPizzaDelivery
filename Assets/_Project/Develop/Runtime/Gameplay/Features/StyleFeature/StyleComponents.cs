using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class StylePoints : IEntityComponent { public ReactiveVariable<float> Value = new(0f); }
    public class StyleRank : IEntityComponent { public ReactiveVariable<StyleRankEnum> Value = new(StyleRankEnum.F); }
    public class StyleMultiplier : IEntityComponent { public ReactiveVariable<float> Value = new(1f); }
    public class MoveFreshness : IEntityComponent { public Dictionary<string, float> LastUsedTimes = new(); }
    public class StyleDecayTimer : IEntityComponent { public ReactiveVariable<float> Value = new(0f); }
    public class MaxStylePoints : IEntityComponent { public float Value; }
    public class MaxStyleRank : IEntityComponent { public StyleRankEnum Value; }
}


