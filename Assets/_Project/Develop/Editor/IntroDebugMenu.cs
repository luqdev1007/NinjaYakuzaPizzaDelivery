using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.Serializers;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor
{
    /// <summary>
    /// После первого прохода IntroSeen становится true и интро больше не
    /// показывается — отлаживать его без сброса флага нельзя.
    /// </summary>
    public static class IntroDebugMenu
    {
        // Путь и ключ зеркалят ProjectContextRegistrations.CreateSaveLoadService
        // и MapDataKeysStorage: в редакторе сейв лежит в Application.dataPath,
        // имя файла — имя типа данных.
        private const string SaveFileName = nameof(PlayerData) + ".json";

        [MenuItem("NYPD/Debug/Replay Intro Next Launch")]
        public static void ReplayIntroNextLaunch()
        {
            string path = Path.Combine(Application.dataPath, SaveFileName);

            if (File.Exists(path) == false)
            {
                Debug.Log($"[IntroDebug] Save not found at '{path}' — intro will play anyway: " +
                          "fresh Reset() gives IntroSeen=false");
                return;
            }

            JsonSerializer serializer = new JsonSerializer();
            PlayerData data = serializer.Deserialize<PlayerData>(File.ReadAllText(path));

            data.IntroSeen = false;

            File.WriteAllText(path, serializer.Serialize(data));
            AssetDatabase.Refresh();

            Debug.Log($"[IntroDebug] IntroSeen reset to false in '{path}'. Остальной прогресс не тронут.");
        }
    }
}
