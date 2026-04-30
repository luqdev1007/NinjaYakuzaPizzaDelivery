using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Services
{
    public interface ILevelStaticDataService
    {
        LevelConfig Config { get; }
        IEnumerable<Vector3> GetChestSpawnPoints();
        IEnumerable<Vector3> GetEnemySpawnPoints();
        string GetLevelName();
        Vector3 GetPlayerStartPosition();
        StageConfig GetStage(int index);
        void Initialize(LevelConfig config);
    }
}