using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore; 
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay
{
    public class RankStylePresenter : IPresenter, IDisposable
    {
        private readonly RankStyleView _view;
        private readonly MainHeroHolderService _heroHolder;
        private readonly List<IDisposable> _disposables = new();

        public RankStylePresenter(RankStyleView view, MainHeroHolderService heroHolder)
        {
            _view = view;
            _heroHolder = heroHolder;
        }

        public void Initialize()
        {
            if (_heroHolder.MainHero != null)
                SubscribeToHero(_heroHolder.MainHero);

            _disposables.Add(_heroHolder.HeroRegistred.Subscribe(SubscribeToHero));
        }

        private void SubscribeToHero(Entity hero)
        {
            var points = hero.GetComponent<StylePoints>();
            var rank = hero.GetComponent<StyleRank>();

            if (points == null || rank == null) 
                return;

            _disposables.Add(points.Value.Subscribe((_, val) => {
                _view.SetProgress(val, 1000f);
                _view.SetPoints(val);
            }));

            _disposables.Add(rank.Value.Subscribe((_, letter) => UpdateVisuals(hero)));

            UpdateVisuals(hero);
        }

        private void UpdateVisuals(Entity hero)
        {
            var rank = hero.GetComponent<StyleRank>().Value.Value;
            _view.SetRank(rank.ToString(), "");
        }

        public void Dispose()
        {
            foreach (var d in _disposables) d.Dispose();
            _disposables.Clear();
        }
    }
}