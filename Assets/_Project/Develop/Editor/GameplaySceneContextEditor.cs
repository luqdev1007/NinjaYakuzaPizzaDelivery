using UnityEditor;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Context;

namespace Assets._Project.Develop.Editor
{
    [CustomEditor(typeof(GameplaySceneContext))]
    public class GameplaySceneContextEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameplaySceneContext context = (GameplaySceneContext)target;

            GUILayout.Space(10);
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("SYNC DATA FROM SCENE/PREFAB", GUILayout.Height(30)))
            {
                context.SyncWithScene();

                EditorUtility.SetDirty(context);

                if (context.gameObject.scene.name == null)
                {
                    AssetDatabase.SaveAssets();
                }
            }

            GUI.backgroundColor = Color.white;
        }
    }
}