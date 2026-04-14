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

        // Новый список для точек спавна
        [SerializeField] private List<Vector3> _enemySpawns = new List<Vector3>();
        public IReadOnlyList<Vector3> EnemySpawns => _enemySpawns;

        public IReadOnlyList<StageConfig> StageConfigs => _stageConfigs;
        public DialogConfig PreparationDialog => _configPrepDialog;


        // Метод для заполнения (вызывается из редактора)
        public void FillSpawnersFromScene()
        {
            _enemySpawns.Clear();
            // Находим все объекты
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

            foreach (var spawner in spawners)
            {
                // Записываем ИМЕННО world position
                _enemySpawns.Add(spawner.transform.position);
            }

            // Обязательно для ScriptableObject!
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets(); // Принудительное сохранение на диск
#endif
            Debug.Log($"[LevelConfig] Успешно сохранено {spawners.Length} точек.");
        }
    }
}