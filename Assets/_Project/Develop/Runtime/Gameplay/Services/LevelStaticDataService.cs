using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Services
{
    public class LevelStaticDataService : ILevelStaticDataService
    {
        private LevelConfig _currentLevelConfig;

        public LevelConfig Config => _currentLevelConfig;

        public void Initialize(LevelConfig config)
        {
            _currentLevelConfig = config;
        }

        public string GetLevelName()
        {
            CheckInitialized();
            return _currentLevelConfig.LevelName;
        }

        public Vector3 GetPlayerStartPosition()
        {
            CheckInitialized();
            return _currentLevelConfig.StartPlayerPosition;
        }

        public IEnumerable<Vector3> GetEnemySpawnPoints()
        {
            CheckInitialized();
            return _currentLevelConfig.EnemySpawns ?? new List<Vector3>();
        }

        public IEnumerable<Vector3> GetChestSpawnPoints()
        {
            CheckInitialized();
            return _currentLevelConfig.SecretChestSpawns ?? new List<Vector3>();
        }

        public StageConfig GetStage(int index)
        {
            CheckInitialized();

            if (index < 0 || index >= _currentLevelConfig.StageConfigs.Count)
            {
                Debug.LogError($"[StaticData] Стадия с индексом {index} " +
                    $"не найдена в конфиге {_currentLevelConfig.LevelName}!");
                return null;
            }

            return _currentLevelConfig.StageConfigs[index];
        }

        private void CheckInitialized()
        {
            if (_currentLevelConfig == null)
                throw new InvalidOperationException("[StaticData] Сервис не инициализирован! " +
                    "Сначала вызови Initialize(LevelConfig).");
        }
    }
}