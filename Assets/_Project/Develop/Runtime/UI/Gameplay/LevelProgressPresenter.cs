using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class LevelProgressPresenter : IPresenter
    {
        private readonly BarWithText _view;
        private readonly LevelProgressService _levelProgressService;

        private IDisposable _progressDisposable;
        private IDisposable _activeDisposable;

        public LevelProgressPresenter(
            BarWithText view,
            LevelProgressService levelProgressService)
        {
            _view = view;
            _levelProgressService = levelProgressService;
        }

        public void Initialize()
        {
            _view.gameObject.SetActive(false);

            _progressDisposable = _levelProgressService.Progress
                .Subscribe((_, newValue) => OnProgressChanged(newValue));

            _activeDisposable = _levelProgressService.IsActive
                .Subscribe((_, isActive) => _view.gameObject.SetActive(isActive));

            OnProgressChanged(_levelProgressService.Progress.Value);
        }

        public void Dispose()
        {
            _progressDisposable?.Dispose();
            _activeDisposable?.Dispose();
        }

        private void OnProgressChanged(float value)
        {
            _view.UpdateSlider(value);
            _view.UpdateText($"{Mathf.RoundToInt(value * 100)}%");
        }
    }
}