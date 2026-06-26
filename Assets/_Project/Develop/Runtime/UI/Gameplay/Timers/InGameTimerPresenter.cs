using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Timers
{
    public class InGameTimerPresenter : IPresenter, IDisposable
    {
        private readonly InGameTimerView _view;
        private readonly TimerService _timerService;
        private readonly InGameTimerFeatureService _timerFeature;
        private readonly float _targetTime;

        private bool _isAlerting;
        private Sequence _alertSequence;
        private readonly List<IDisposable> _disposables = new();

        public InGameTimerPresenter(
            InGameTimerView view,
            TimerService timerService,
            InGameTimerFeatureService timerFeature,
            float targetTime)
        {
            _view = view;
            _timerService = timerService;
            _timerFeature = timerFeature;
            _targetTime = targetTime;
        }

        public void Initialize()
        {
            _view.Group.alpha = 0;

            _disposables.Add(_timerService.CurrentTime.Subscribe(OnTimeChanged));
            _disposables.Add(_timerService.CooldownEnded.Subscribe(OnTimerFinished));
        }

        public void ShowAndStart()
        {
            _isAlerting = false;
            _alertSequence?.Kill();
            _view.transform.localScale = Vector3.one;

            _view.Group.DOFade(1, 0.5f).SetUpdate(true);
            _view.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f).SetUpdate(true);

            _timerService.Restart();
        }

        public void Hide()
        {
            _view.Group.DOFade(0, 0.5f).SetUpdate(true);
            _timerService.Stop();
            _alertSequence?.Kill();
        }

        private void OnTimeChanged(float oldValue, float timeLeft)
        {
            float elapsed = Mathf.Max(0, _targetTime - timeLeft);
            float progress = Mathf.Clamp01(elapsed / _targetTime);

            _view.SetText($"{elapsed:00.00}s");
            _view.SetProgress(progress);

            UpdateVisuals(progress);

            if (progress > 0.75f && !_isAlerting)
                StartAlertAnimation();
        }

        private void UpdateVisuals(float progress)
        {
            Color textMin, textMax, bgColor;

            if (progress <= 0.33f)
            {
                textMin = Color.green; textMax = Color.yellow;
                bgColor = new Color(0, 1, 0, 0.2f);
            }
            else if (progress <= 0.66f)
            {
                textMin = Color.yellow; textMax = new Color(1, 0.5f, 0);
                bgColor = new Color(1, 1, 0, 0.2f);
            }
            else
            {
                textMin = new Color(1, 0.5f, 0); textMax = Color.red;
                bgColor = new Color(1, 0, 0, 0.2f);
            }

            _view.UpdateColors(textMin, textMax, bgColor);
        }

        private void StartAlertAnimation()
        {
            _isAlerting = true;
            _alertSequence = DOTween.Sequence()
                .Append(_view.transform.DOScale(1.1f, 0.3f))
                .Append(_view.transform.DOScale(1.0f, 0.3f))
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void OnTimerFinished()
        {
            _alertSequence?.Kill();

            _view.transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.5f).SetUpdate(true);
            _view.Group.DOFade(0, 1f).SetDelay(2f).SetUpdate(true);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();

            _alertSequence?.Kill();
            _view.transform.DOKill();
            _view.Group.DOKill();

            _timerService.Stop();
        }
    }
}