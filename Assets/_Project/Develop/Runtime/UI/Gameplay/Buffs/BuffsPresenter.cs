using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Buffs
{
    public class BuffsPresenter : IPresenter, IHudPausable
    {
        private readonly BuffsView _view;
        private readonly MainHeroHolderService _mainHeroHolderService;

        private readonly Dictionary<ActiveBuff, BuffPresenter> _presenters = new();

        private ActiveBuffsList _activeBuffsList;
        private IDisposable _heroRegistredDisposable;

        public BuffsPresenter(BuffsView view, MainHeroHolderService mainHeroHolderService)
        {
            _view = view;
            _mainHeroHolderService = mainHeroHolderService;
        }

        public void Initialize()
        {
            if (_mainHeroHolderService.MainHero != null)
            {
                BindToHero(_mainHeroHolderService.MainHero);
            }
            else
            {
                _heroRegistredDisposable = _mainHeroHolderService.HeroRegistred.Subscribe(OnHeroRegistred);
            }
        }

        private void OnHeroRegistred(Entity hero)
        {
            BindToHero(hero);
        }

        private void BindToHero(Entity hero)
        {
            _activeBuffsList = hero.ActiveBuffsC.Value;

            foreach (ActiveBuff activeBuff in _activeBuffsList.Elements)
            {
                CreatePresenterFor(activeBuff);
            }

            _activeBuffsList.Added += OnBuffAdded;
            _activeBuffsList.Removed += OnBuffRemoved;
        }

        private void OnBuffAdded(ActiveBuff activeBuff)
        {
            CreatePresenterFor(activeBuff);
        }

        private void OnBuffRemoved(ActiveBuff activeBuff)
        {
            if (_presenters.TryGetValue(activeBuff, out BuffPresenter presenter) == false)
            {
                return;
            }

            presenter.View.PlayDisappearAndDestroy();
            presenter.Dispose();
            _presenters.Remove(activeBuff);
        }

        private void CreatePresenterFor(ActiveBuff activeBuff)
        {
            BuffView view = _view.CreateBuffView();

            BuffPresenter presenter = new BuffPresenter(view, activeBuff);
            presenter.Initialize();

            _presenters.Add(activeBuff, presenter);
        }

        public void Hide()
        {
            _view.Hide();
        }

        public void Dispose()
        {
            _heroRegistredDisposable?.Dispose();

            if (_activeBuffsList != null)
            {
                _activeBuffsList.Added -= OnBuffAdded;
                _activeBuffsList.Removed -= OnBuffRemoved;
            }

            foreach (KeyValuePair<ActiveBuff, BuffPresenter> pair in _presenters)
            {
                pair.Value.Dispose();
            }

            _presenters.Clear();
        }
    }
}