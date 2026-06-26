using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class StageProviderService : IDisposable
    {
        private ReactiveVariable<int> _currentStageNumber = new(0);
        private ReactiveVariable<StageResults> _currentStageResult = new(StageResults.Uncompleted);

        private LevelConfig _levelConfig;
        private StagesFactory _stagesFactory;

        private IStage _currentStage;

        private IDisposable _stageEndedDisposable;

        public StageProviderService(
            LevelConfig levelConfig,
            StagesFactory stagesFactory)
        {
            _levelConfig = levelConfig;
            _stagesFactory = stagesFactory;
        }

        public IReadOnlyVariable<int> CurrentStageNumber => _currentStageNumber;
        public IReadOnlyVariable<StageResults> CurrentStageResult => _currentStageResult;

        public int StagesCount => _levelConfig.StageConfigs.Count;

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

            _currentStage = _stagesFactory.Create(_levelConfig.StageConfigs[_currentStageNumber.Value - 1]);

            StartCurrent();
            _currentStage.Start();
        }

        private void OnStageCompleted()
        {
            _currentStageResult.Value = StageResults.Completed;
        }

        public void PrepareFirstStage()
        {
            if (_currentStage != null)
                return;

            if (_levelConfig.StageConfigs == null || _levelConfig.StageConfigs.Count == 0)
                throw new InvalidOperationException("LevelConfig has no StageConfigs — level cannot be completed.");

            _currentStageNumber.Value = 1;
            _currentStageResult.Value = StageResults.Uncompleted;
            _currentStage = _stagesFactory.Create(_levelConfig.StageConfigs[0]);

            StartCurrent();
            _currentStage.Start();
        }

        public void StartCurrent()
        {
            _stageEndedDisposable?.Dispose();
            _stageEndedDisposable = _currentStage.Completed.Subscribe(OnStageCompleted);
        }

        public void UpdateCurrent(float deltaTime) => _currentStage?.Update(deltaTime);

        public void CleanupCurrent()
        {
            _stageEndedDisposable?.Dispose();
            _stageEndedDisposable = null;

            _currentStage?.Cleanup();
        }

        public void Dispose()
        {
            _currentStage?.Dispose();
            _stageEndedDisposable?.Dispose();
        }
    }
}