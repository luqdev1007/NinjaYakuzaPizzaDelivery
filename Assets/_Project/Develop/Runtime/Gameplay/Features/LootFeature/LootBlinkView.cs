using UnityEngine;
using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class LootBlinkView : EntityView
    {
        [SerializeField] private SpriteRenderer[] _renderers;
        [SerializeField] private float _blinkThreshold = 1.5f;

        private Entity _linkedEntity;
        private IDisposable _lootLifeTimeDisposable; 
        private bool _isBlinking;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;

            _lootLifeTimeDisposable = _linkedEntity.LootCurrentLifeTime.Subscribe(OnTimerChanged);
        }

        private void OnTimerChanged(float oldValue, float currentTime)
        {
            _isBlinking = currentTime <= _blinkThreshold && currentTime > 0;

            if (!_isBlinking)
            {
                ResetAlpha();
            }
        }

        private void Update()
        {
            if (_isBlinking)
            {
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * 15f));
                SetAlpha(alpha);
            }
        }

        private void SetAlpha(float alpha)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) 
                    continue;

                Color color = _renderers[i].color;
                color.a = alpha;
                _renderers[i].color = color;
            }
        }

        private void ResetAlpha() => SetAlpha(1f);

        public override void Cleanup(Entity entity)
        {
            _isBlinking = false;

            if (_lootLifeTimeDisposable != null)
            {
                _lootLifeTimeDisposable.Dispose();
                _lootLifeTimeDisposable = null;
            }

            ResetAlpha();
            _linkedEntity = null;
            base.Cleanup(entity);
        }
    }
}