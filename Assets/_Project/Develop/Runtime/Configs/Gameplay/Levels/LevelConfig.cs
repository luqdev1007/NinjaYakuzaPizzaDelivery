using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/New Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("General Info")]
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelNumber { get; private set; }
        [field: SerializeField] public Sprite LevelIcon { get; private set; }
        [field: SerializeField] public GameObject LevelPrefab { get; private set; }

        [Header("Flow & Logic")]
        [SerializeField] private List<StageConfig> _stageConfigs;
        [SerializeField] private DialogConfig _configPrepDialog;

        [field: SerializeField] public float TargetTime { get; private set; }
        [field: SerializeField] public float StyleStarThreshold { get; private set; }

        [Header("Static Positions")]
        [field: SerializeField] public Vector3 StartPlayerPosition { get; private set; }
        [field: SerializeField] public Vector3 FinalPointPosition { get; private set; }

        [Header("Spawn Collections")]
        [SerializeField] private List<Vector3> _enemySpawns = new();
        [SerializeField] private List<Vector3> _secretChestSpawns = new();

        public IReadOnlyList<StageConfig> StageConfigs => _stageConfigs;
        public DialogConfig PreparationDialog => _configPrepDialog;
        public IReadOnlyList<Vector3> EnemySpawns => _enemySpawns;
        public IReadOnlyList<Vector3> SecretChestSpawns => _secretChestSpawns;

        public void ApplyBakedData(Vector3 start, Vector3 finish, List<Vector3> enemies, List<Vector3> chests)
        {
            StartPlayerPosition = start;
            FinalPointPosition = finish;
            _enemySpawns = enemies;
            _secretChestSpawns = chests;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}