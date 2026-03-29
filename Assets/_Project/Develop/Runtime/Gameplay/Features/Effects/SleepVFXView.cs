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
        // Transform, который мы будем "пульсировать" (дыхание)
        [SerializeField] private Transform _breatheContainer;
        // Партикл-система с Z
        [SerializeField] private ParticleSystem _zzzParticles;

        [Header("Breathe Effect")]
        [SerializeField] private float _breatheSpeed = 2f;
        [SerializeField] private float _breatheAmount = 0.05f; // Амплитуда (5%)
        [SerializeField] private Color _sleepColor = new Color(0.4f, 0.4f, 0.8f, 0.7f); // Тусклый синеватый

        private Color _initialColor;
        private Vector3 _initialScale;
        private IReadOnlyVariable<bool> _isAsleep;
        private IDisposable _disposable;
        private float _timer;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _initialColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
            _initialScale = _breatheContainer != null ? _breatheContainer.localScale : Vector3.one;

            _isAsleep = entity.IsAsleep;
            // Подписываемся на изменение состояния сна.
            _disposable = _isAsleep.Subscribe(OnSleepChanged);

            // Сразу устанавливаем визуальное состояние при старте (если враг уже спит).
            OnSleepChanged(false, _isAsleep.Value);
        }

        private void Update()
        {
            // Если сущность не спит, эффект дыхания не нужен.
            if (_isAsleep == null || !_isAsleep.Value) return;

            // Эффект дыхания через Sin.
            _timer += Time.deltaTime * _breatheSpeed;
            float pulse = Mathf.Sin(_timer) * _breatheAmount;

            if (_breatheContainer != null)
            {
                // Применяем пульсацию к масштабу.
                _breatheContainer.localScale = _initialScale + new Vector3(pulse, pulse, 0);
            }
        }

        private void OnSleepChanged(bool oldValue, bool value)
        {
            _timer = 0f;

            if (value)
            {
                // --- ВРАГ УСНУЛ ---
                if (_spriteRenderer != null)
                {
                    // Делаем его заметно синим и темным (0.2f - это очень темно)
                    _spriteRenderer.color = new Color(0.2f, 0.4f, 1.0f, 1.0f);
                }

                if (_zzzParticles != null) _zzzParticles.Play(true);

                // Давай добавим микро-скейл, чтобы он "сжался" когда спит
                if (_breatheContainer != null) _breatheContainer.localScale = _initialScale * 0.9f;
            }
            else
            {
                // --- ВРАГ ПРОСНУЛСЯ ---
                if (_spriteRenderer != null) _spriteRenderer.color = _initialColor;
                if (_breatheContainer != null) _breatheContainer.localScale = _initialScale;
                if (_zzzParticles != null) _zzzParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            // Отписываемся, чтобы не было утечек памяти.
            _disposable?.Dispose();
        }
    }
}