using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay
{
    public class RankStylePresenter : IPresenter
    {
        private readonly RankStyleView _view;
        private readonly RankStyleService _styleService;
        private readonly List<IDisposable> _disposables = new();

        public RankStylePresenter(RankStyleView view, RankStyleService styleService)
        {
            _view = view;
            _styleService = styleService;
        }

        public void Initialize()
        {
            // Оставляем одну подписку на всё, что связано с очками
            _disposables.Add(_styleService.CurrentPoints.Subscribe((_, points) =>
            {
                // 1000f — это заглушка, позже можно брать из текущего ранга конфига
                _view.SetProgress(points, 1000f);
                _view.SetPoints(points);
            }));

            _disposables.Add(_styleService.CurrentLetter.Subscribe((_, letter) => UpdateVisuals()));
            _disposables.Add(_styleService.CurrentPrefix.Subscribe((_, prefix) => UpdateVisuals()));

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _view.SetRank(_styleService.CurrentLetter.Value, _styleService.CurrentPrefix.Value);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }
    }
}