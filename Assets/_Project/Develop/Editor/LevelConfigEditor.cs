using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.Configs
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelConfig config = (LevelConfig)target;

            GUILayout.Space(15);

            // Кнопка для старта и финиша
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Collect Start & Finish Points", GUILayout.Height(30)))
            {
                config.FillStartAndFinishPoints();
            }

            GUILayout.Space(5);

            // Кнопка для врагов
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Collect Enemy Spawners", GUILayout.Height(30)))
            {
                config.FillSpawnersFromScene();
            }

            GUILayout.Space(5);

            // Кнопка для секретных сундуков
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Collect Secret Chests", GUILayout.Height(30)))
            {
                config.FillSecretChestsFromScene();
            }

            GUI.backgroundColor = Color.white;
        }
    }
}