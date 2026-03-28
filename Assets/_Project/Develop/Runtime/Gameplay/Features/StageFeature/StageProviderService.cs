using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
    public class StageProviderService : IDisposable
    {
        private ReactiveVariable<int> _currentStageNumber = new();
        private ReactiveVariable<StageResults> _currentStageResult = new();

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
            // Проверка: есть ли вообще следующий стейдж
            if (HasNextStage() == false)
            {
                // Вместо ошибки просто логируем или выходим, 
                // так как это может быть последний стейдж уровня
                Debug.Log("No more stages to switch to.");
                return;
            }

            if (_currentStage != null)
                CleanupCurrent();

            _currentStageNumber.Value++;
            _currentStageResult.Value = StageResults.Uncompleted;

            _currentStage = _stagesFactory.Create(_levelConfig.StageConfigs[_currentStageNumber.Value - 1]);
        }

        private void OnStageCompleted()
        {
            _currentStageResult.Value = StageResults.Completed;

            // Если это был последний стейдж, не пытаемся переключаться дальше автоматически
            if (HasNextStage())
            {
                // SwitchToNext(); // Если у тебя логика автоматического перехода
            }
        }

        public void PrepareFirstStage()
        {
            if (_currentStage != null) 
                return;

            _currentStageNumber.Value = 1;
            _currentStageResult.Value = StageResults.Uncompleted;
            _currentStage = _stagesFactory.Create(_levelConfig.StageConfigs[0]);

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
