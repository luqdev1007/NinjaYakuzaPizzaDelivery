using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Effects
{
    public class SleepVFXView : EntityView
    {
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private ParticleSystem _zzzParticles;

        [Header("Breathe Effect")]
        [SerializeField] private float _breatheSpeed = 2f;
        [SerializeField] private float _breatheAmount = 0.1f; // На сколько меняется масштаб
        [SerializeField] private Color _sleepColor = new Color(0.5f, 0.5f, 1f, 0.6f); // Блекло-синий

        private Color _initialColor;
        private Vector3 _initialScale;
        private IReadOnlyVariable<bool> _isAsleep;
        private IDisposable _disposable;
        private float _timer;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _initialColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
            _initialScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;

            _isAsleep = entity.IsAsleep;
            _disposable = _isAsleep.Subscribe(OnSleepChanged);

            // Сразу проверяем текущее состояние
            OnSleepChanged(false, _isAsleep.Value);
        }

        private void Update()
        {
            if (_isAsleep == null || !_isAsleep.Value) return;

            // Эффект дыхания (Scale)
            _timer += Time.deltaTime * _breatheSpeed;
            float pulse = Mathf.Sin(_timer) * _breatheAmount;

            if (_viewContainer != null)
            {
                _viewContainer.localScale = _initialScale + new Vector3(pulse, pulse, 0);
            }
        }

        private void OnSleepChanged(bool oldValue, bool value)
        {
            _timer = 0f;

            if (value)
            {
                // Становимся блеклыми
                if (_spriteRenderer != null) _spriteRenderer.color = _sleepColor;
                if (_zzzParticles != null) _zzzParticles.Play();
            }
            else
            {
                // Возвращаем как было
                if (_spriteRenderer != null) _spriteRenderer.color = _initialColor;
                if (_viewContainer != null) _viewContainer.localScale = _initialScale;
                if (_zzzParticles != null) _zzzParticles.Stop();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _disposable?.Dispose();
        }
    }
}