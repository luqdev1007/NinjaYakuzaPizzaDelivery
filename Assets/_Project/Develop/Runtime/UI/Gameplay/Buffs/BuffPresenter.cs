using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Buffs
{
    public class BuffPresenter : IPresenter
    {
        private readonly BuffView _view;
        private readonly ActiveBuff _activeBuff;

        private IDisposable _remainingTimeDisposable;

        public BuffPresenter(BuffView view, ActiveBuff activeBuff)
        {
            _view = view;
            _activeBuff = activeBuff;
        }

        public BuffView View => _view;

        public void Initialize()
        {
            _view.SetIcon(_activeBuff.Icon);
            UpdateView(_activeBuff.RemainingTime.Value);
            _view.PlayAppear();

            _remainingTimeDisposable = _activeBuff.RemainingTime.Subscribe(OnRemainingTimeChanged);
        }

        private void OnRemainingTimeChanged(float previousValue, float newValue)
        {
            if (newValue > previousValue + 0.01f)
            {
                _view.PlayExtendedPulse();
            }

            UpdateView(newValue);
        }

        private void UpdateView(float remainingTime)
        {
            float normalized = _activeBuff.MaxDuration > 0f
                ? remainingTime / _activeBuff.MaxDuration
                : 0f;

            _view.SetProgress(normalized);
            _view.SetTimeText($"{Mathf.Max(0f, remainingTime):0.0}s");
        }

        public void Dispose()
        {
            _remainingTimeDisposable?.Dispose();
        }
    }
}