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
            // Рисуем стандартный инспектор
            DrawDefaultInspector();

            LevelConfig config = (LevelConfig)target;

            GUILayout.Space(15);
            GUI.backgroundColor = Color.cyan;

            if (GUILayout.Button("Collect Spawners from Scene", GUILayout.Height(30)))
            {
                config.FillSpawnersFromScene();
            }

            GUI.backgroundColor = Color.white;
        }
    }
}