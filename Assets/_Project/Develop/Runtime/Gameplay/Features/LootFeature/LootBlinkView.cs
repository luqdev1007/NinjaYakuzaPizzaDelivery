using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Loot
{
    public class LootBlinkView : EntityView
    {
        [SerializeField] private SpriteRenderer[] _renderers;
        [SerializeField] private float _blinkThreshold = 1.5f;

        private IDisposable _timerDisposable;
        private bool _isBlinking;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _timerDisposable = entity.AutoDeleteCurrentTime.Subscribe(OnTimerChanged);
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
                if (_renderers[i] == null) continue;
                Color color = _renderers[i].color;
                color.a = alpha;
                _renderers[i].color = color;
            }
        }

        private void ResetAlpha() => SetAlpha(1f);

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isBlinking = false;
            _timerDisposable?.Dispose();
            ResetAlpha();
        }
    }
}