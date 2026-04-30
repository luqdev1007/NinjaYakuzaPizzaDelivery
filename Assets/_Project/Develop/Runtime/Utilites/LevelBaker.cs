using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites
{
    public static class LevelBaker
    {
        public static void Bake(LevelConfig config)
        {
            Undo.RecordObject(config, $"Bake Level Data: {config.name}");

            var enemies = GameObject.FindGameObjectsWithTag("Spawner")
                .Select(s => s.transform.position)
                .ToList();

            var chests = GameObject.FindGameObjectsWithTag("SecretChest")
                .Select(c => c.transform.position)
                .ToList();

            var startPoint = GameObject.FindWithTag("StartPoint");
            var finishPoint = GameObject.FindWithTag("FinishPoint");

            Vector3 startPos = startPoint != null ? startPoint.transform.position : Vector3.zero;
            Vector3 finishPos = finishPoint != null ? finishPoint.transform.position : Vector3.zero;

            if (string.IsNullOrEmpty(config.LevelName))
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                Debug.Log($"[Baker] Имя уровня не задано, использую имя сцены: {scene.name}");
            }

            config.ApplyBakedData(startPos, finishPos, enemies, chests);

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>[Baker]</color> Уровень <b>{config.LevelName}</b> успешно обновлен! " +
                      $"(Врагов: {enemies.Count}, Сундуков: {chests.Count})");
        }
    }
}