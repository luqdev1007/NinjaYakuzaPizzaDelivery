using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class ActiveBuff
    {
        public ActiveBuff(string id, IBuffEffect effect, float duration)
        {
            Id = id;
            Effect = effect;
            RemainingTime = new ReactiveVariable<float>(duration);
        }

        public string Id { get; }
        public IBuffEffect Effect { get; }
        public ReactiveVariable<float> RemainingTime { get; }
    }
}