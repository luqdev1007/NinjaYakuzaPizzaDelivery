using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory
{
    public abstract class ConsumableConfig : ScriptableObject
    {
        public string Name;
        public Sprite Icon;

        public abstract void Use(Entity user, IThrowableBehaviourFactory factory);
    }
}
