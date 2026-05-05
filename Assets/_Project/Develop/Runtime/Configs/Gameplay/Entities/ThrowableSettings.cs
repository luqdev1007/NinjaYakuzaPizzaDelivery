using Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory; 
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [Serializable]
    public class ThrowableSettings
    {
        [field: SerializeField] public ThrowableItemConfig GrappleItem { get; private set; }
        [field: SerializeField] public ThrowableItemConfig ShurikenItem { get; private set; }
        [field: SerializeField] public ThrowableItemConfig SleepDartItem { get; private set; }
    }
}