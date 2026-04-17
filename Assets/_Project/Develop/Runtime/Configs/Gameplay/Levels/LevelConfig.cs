using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/New Level Config", fileName = "LevelConfig", order = 54)]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private List<StageConfig> _stageConfigs;
        [SerializeField] private DialogConfig _configPrepDialog;

        [field: SerializeField] public float TargetTime { get; private set; }
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelNumber { get; private set; }
        [field: SerializeField] public Sprite LevelIcon { get; private set; }
        [field: SerializeField] public Vector3 FinalPointPosition { get; private set; }
        [field: SerializeField] public Vector3 StartPlayerPosition { get; private set; }
        [field: SerializeField] public GameObject LevelPrefab { get; private set; }

        [Header("Spawn Points")]
        [SerializeField] private List<Vector3> _enemySpawns = new List<Vector3>();
        public IReadOnlyList<Vector3> EnemySpawns => _enemySpawns;

        [SerializeField] private List<Vector3> _secretChestSpawns = new List<Vector3>();
        public IReadOnlyList<Vector3> SecretChestSpawns => _secretChestSpawns;

        public IReadOnlyList<StageConfig> StageConfigs => _stageConfigs;
        public DialogConfig PreparationDialog => _configPrepDialog;

        public void FillSpawnersFromScene()
        {
            _enemySpawns.Clear();
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

            foreach (var spawner in spawners)
            {
                _enemySpawns.Add(spawner.transform.position);
            }

            MarkDirty();
            Debug.Log($"[LevelConfig] Сохранено {spawners.Length} врагов.");
        }

        public void FillSecretChestsFromScene()
        {
            _secretChestSpawns.Clear();
            GameObject[] chests = GameObject.FindGameObjectsWithTag("SecretChest");

            foreach (var chest in chests)
            {
                _secretChestSpawns.Add(chest.transform.position);
            }

            MarkDirty();
            Debug.Log($"[LevelConfig] Сохранено {chests.Length} сундуков.");
        }

        public void FillStartAndFinishPoints()
        {
            GameObject startPoint = GameObject.FindWithTag("StartPoint");
            GameObject finishPoint = GameObject.FindWithTag("FinishPoint");

            if (startPoint != null)
                StartPlayerPosition = startPoint.transform.position;
            else
                Debug.LogWarning("[LevelConfig] Объект с тегом 'StartPoint' не найден!");

            if (finishPoint != null)
                FinalPointPosition = finishPoint.transform.position;
            else
                Debug.LogWarning("[LevelConfig] Объект с тегом 'FinishPoint' не найден!");

            MarkDirty();
            Debug.Log("[LevelConfig] Точки старта и финиша обновлены.");
        }

        private void MarkDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}