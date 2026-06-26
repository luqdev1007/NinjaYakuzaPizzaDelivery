using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay
{
    public class RankStylePresenter : IPresenter, IHudPausable
    {
        private readonly RankStyleView _view;
        private readonly RankStyleService _styleService;
        private readonly List<IDisposable> _disposables = new();

        private float _previousPoints;

        public RankStylePresenter(RankStyleView view, RankStyleService styleService)
        {
            _view = view;
            _styleService = styleService;
        }

        public void Initialize()
        {
            _disposables.Add(_styleService.CurrentPoints.Subscribe((_, points) =>
            {
                float delta = points - _previousPoints;
                if (delta > 0.5f)
                {
                    _view.PlayPointsGained(delta);
                }
                _previousPoints = points;

                var bounds = _styleService.GetCurrentSubRangeBounds();
                _view.SetProgress(points, bounds.Floor, bounds.Ceiling);
                _view.SetPoints(points);
            }));

            _disposables.Add(_styleService.CurrentLetter.Subscribe((_, letter) => UpdateVisuals()));
            _disposables.Add(_styleService.CurrentPrefix.Subscribe((_, prefix) => UpdateVisuals()));

            _disposables.Add(_styleService.DecayWarning.Subscribe((_, value) => _view.SetDecayWarning(value)));
            _disposables.Add(_styleService.IsDecaying.Subscribe((_, isDecaying) => _view.SetDecayActive(isDecaying)));

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            string letter = _styleService.CurrentLetter.Value;
            Color accent = _styleService.GetAccentColor(letter);
            _view.SetRank(letter, _styleService.CurrentPrefix.Value, accent);
        }

        public void Hide()
        {
            _view.Hide();
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }
    }
}