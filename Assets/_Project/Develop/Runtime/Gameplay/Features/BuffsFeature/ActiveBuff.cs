using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class ActiveBuff
    {
        public ActiveBuff(string id, IBuffEffect effect, float duration, Sprite icon)
        {
            Id = id;
            Effect = effect;
            Icon = icon;
            MaxDuration = duration;
            RemainingTime = new ReactiveVariable<float>(duration);
        }

        public string Id { get; }
        public IBuffEffect Effect { get; }
        public Sprite Icon { get; }
        public float MaxDuration { get; private set; }
        public ReactiveVariable<float> RemainingTime { get; }

        public void Extend(float additionalDuration)
        {
            MaxDuration += additionalDuration;
            RemainingTime.Value += additionalDuration;
        }
    }
}