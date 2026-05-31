using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Props
{
    [CreateAssetMenu(fileName = "NewPropConfig", menuName = "Configs/Props/PropConfig")]
    public class PropConfig : ScriptableObject
    {
        [Header("Life Cycle")]
        [SerializeField] private float _maxHealth = 10f;

        public float MaxHealth => _maxHealth;
    }
}
