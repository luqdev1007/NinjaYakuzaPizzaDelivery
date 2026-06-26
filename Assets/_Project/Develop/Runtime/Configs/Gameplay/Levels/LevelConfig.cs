using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages; 
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/New Level Config", fileName = "LevelConfig", order = 54)]
    public class LevelConfig : ScriptableObject
    {
        [Header("Meta Info")]
        [field: SerializeField] public int LevelNumber { get; private set; }
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public Sprite LevelIcon { get; private set; }

        [Header("Core Assets")]
        [field: SerializeField] public GameObject LevelPrefab { get; private set; }
        [field: SerializeField] public DialogConfig StartLevelDialogConfig { get; private set; }

        [Header("Stages")]
        [SerializeField] private List<StageConfig> _stageConfigs;
        public IReadOnlyList<StageConfig> StageConfigs => _stageConfigs;

        [Header("Balance & Rules")]
        [field: SerializeField] public float TargetTime { get; private set; }
        [field: SerializeField] public float StyleStarThreshold { get; private set; }
        [field: SerializeField] public int CurrencyStarGoldThreshold { get; private set; }
        [field: SerializeField] public int CurrencyStarShardThreshold { get; private set; }

        [field: SerializeField] public Vector3 StartPlayerPosition { get; set; }
    }
}