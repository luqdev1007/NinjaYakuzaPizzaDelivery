using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Explosions
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Explosions/New Explosions List Config", fileName = "ExplosionsListConfig", order = 54)]
    public class ExplosionsListConfig : ScriptableObject
    {
        [SerializeField] private List<ExplosionConfig> _explosions;

        public IReadOnlyList<ExplosionConfig> ExplosionsList => _explosions;

        public ExplosionConfig GetBy(ExplosionType type) => _explosions.Find(i => i.Type == type);
    }
}
