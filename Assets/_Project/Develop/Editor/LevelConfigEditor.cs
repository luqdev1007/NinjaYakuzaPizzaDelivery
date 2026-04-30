using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilites;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelConfig config = (LevelConfig)target;

            GUILayout.Space(20);
            GUI.backgroundColor = Color.cyan;

            if (GUILayout.Button("BAKE LEVEL DATA", GUILayout.Height(40)))
            {
                LevelBaker.Bake(config);
            }

            GUI.backgroundColor = Color.white;
        }
    }
}