using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Stages
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Stages/ClearAllEnemiesStage", fileName = "New Clear All Enemies Stage Config")]
    public class ClearAllEnemiesStageConfig : StageConfig
    {
        [SerializeField] private List<EnemyItemConfig> _enemyItems;

        public IReadOnlyList<EnemyItemConfig> EnemyItems => _enemyItems;
    }

    [Serializable]
    public class EnemyItemConfig
    {
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
        [field: SerializeField] public EntityConfig EnemyConfig { get; private set; }
    }
}
