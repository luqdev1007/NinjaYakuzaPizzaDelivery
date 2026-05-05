using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Potion")]
    public class PotionItemConfig : ConsumableConfig
    {
        public float SpeedBonus = 2f;

        public override void Use(Entity user, IThrowableBehaviourFactory factory)
        {
            // user.MoveSpeed.Value += SpeedBonus;
            Debug.Log("Выпил зелье, фабрика не пригодилась!");
        }
    }
}