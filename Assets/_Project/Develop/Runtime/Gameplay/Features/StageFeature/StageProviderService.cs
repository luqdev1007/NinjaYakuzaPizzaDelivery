using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.Services; // Новый сервис
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class StageProviderService : IDisposable
    {
        private ReactiveVariable<int> _currentStageNumber = new();
        private ReactiveVariable<StageResults> _currentStageResult = new();

        private readonly ILevelStaticDataService _levelData;
        private readonly StagesFactory _stagesFactory;

        private IStage _currentStage;
        private IDisposable _stageEndedDisposable;

        public StageProviderService(
            ILevelStaticDataService levelData,
            StagesFactory stagesFactory)
        {
            _levelData = levelData;
            _stagesFactory = stagesFactory;
        }

        public IReadOnlyVariable<int> CurrentStageNumber => _currentStageNumber;
        public IReadOnlyVariable<StageResults> CurrentStageResult => _currentStageResult;

        public int StagesCount => _levelData.Config.StageConfigs.Count;

        public bool HasNextStage() => CurrentStageNumber.Value < StagesCount;

        public void SwitchToNext()
        {
            if (HasNextStage() == false)
            {
                Debug.Log("No more stages to switch to.");
                return;
            }

            if (_currentStage != null)
                CleanupCurrent();

            _currentStageNumber.Value++;
            _currentStageResult.Value = StageResults.Uncompleted;

            // Берем конфиг из обновляемого сервиса данных
            var nextConfig = _levelData.Config.StageConfigs[_currentStageNumber.Value - 1];
            _currentStage = _stagesFactory.Create(nextConfig);
        }

        private void OnStageCompleted()
        {
            _currentStageResult.Value = StageResults.Completed;
        }

        public void PrepareFirstStage()
        {
            if (_currentStage != null)
                return;

            _currentStageNumber.Value = 1;
            _currentStageResult.Value = StageResults.Uncompleted;

            _currentStage = _stagesFactory.Create(_levelData.Config.StageConfigs[0]);

            _currentStage.Start();
        }

        public void StartCurrent()
        {
            _stageEndedDisposable?.Dispose();
            _stageEndedDisposable = _currentStage.Completed.Subscribe(OnStageCompleted);
        }

        public void UpdateCurrent(float deltaTime) => _currentStage.Update(deltaTime);

        public void CleanupCurrent() => _currentStage.Cleanup();

        public void Dispose()
        {
            _currentStage?.Dispose();
            _stageEndedDisposable?.Dispose();
        }
    }
}