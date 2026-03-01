using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using Assets._Project.Develop.Runtime.Gameplay.Features.Mines;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly MinePlacementService _minePlacementService;

        private List<IDisposable> _disposables = new();

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view, 
            MinePlacementService minePlacementService)
        {
            _view = view;
            _minePlacementService = minePlacementService;
        }

        public void Initialize()
        {
            _view.Init();

            _view.UpgradesPanelView.CreateMineButton.OnHoldComplete += CreateMine;
            
            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
            _view.UpgradesPanelView.CreateMineButton.OnHoldComplete -= CreateMine;

            foreach (var disposable in _disposables)
                disposable.Dispose();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _disposables.Clear();
        }

        public void HideUI()
        {
            _view.HealthView.Hide();
        }

        public void SubscribeHealthViewToEntity(Entity entity)
        {
            _view.HealthView.Init(entity.CurrentHealth, entity.MaxHealth);
            _view.HealthView.Show();
        }

        public void ShowAnnouncement(string header, string subheader = "")
        {
            _view.AnouncementView.SetText(header, subheader);
            _view.AnouncementView.Show();
        }

        public void ShowPreperationTimer(TimerService timerService)
        {
            _view.PrepTimerView.Show();

            _disposables.Add(timerService.CurrentTime.Subscribe(OnTimerChanged));
            _disposables.Add(timerService.CooldownEnded.Subscribe(OnTimerEnded));

            timerService.Restart();
        }

        public void HideUpgradesPanel()
        {
            _view.UpgradesPanelView.Hide();
        }

        public void ShowUpgradesPanel()
        {
            _view.UpgradesPanelView.Show();
        }

        private void CreateMine()
        {
            if (_minePlacementService.IsPlacing)
                _minePlacementService.CancelPlacement();
            else
                _minePlacementService.BeginPlacement();
        }

        private void OnTimerEnded()
        {
            _view.PrepTimerView.Hide();
        }

        private void OnTimerChanged(float arg1, float timeLeft)
        {
            _view.TimerText.text = ((int)timeLeft).ToString();
        }
    }
}