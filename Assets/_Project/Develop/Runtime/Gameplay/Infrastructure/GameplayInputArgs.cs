using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.SceneManagement
{
    public class GameplayInputArgs : IInputSceneArgs
    {
        public GameplayInputArgs(int levelNumber, Vector3 levelSpawnPointPosition)
        {
            LevelNumber = levelNumber;
            FinalPointSpawnPosition = levelSpawnPointPosition;
        }

        public int LevelNumber { get; private set; }
        public Vector3 FinalPointSpawnPosition { get; private set; }
    }
}
