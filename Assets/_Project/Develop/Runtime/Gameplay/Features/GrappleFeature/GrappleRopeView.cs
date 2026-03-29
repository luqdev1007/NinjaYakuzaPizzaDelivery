using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    [RequireComponent(typeof(LineRenderer))]
    public class GrappleRopeView : EntityView
    {
        [Header("References")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _ropeOrigin;

        [Header("Settings")]
        [SerializeField] private int _precision = 20; // Сколько сегментов в веревке
        [SerializeField] private float _waveAmplitude = 0.5f; // Высота волны при выстреле
        [SerializeField] private float _waveFrequency = 2f; // Частота волн
        [SerializeField] private AnimationCurve _waveCurve; // Кривая затухания волны (от 1 до 0)

        private IReadOnlyVariable<bool> _isThrowing;
        private IDisposable _isThrowingDisposable;
        private Transform _hookTransform;
        private float _animationTime;

        private void OnValidate()
        {
            _lineRenderer ??= GetComponent<LineRenderer>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isThrowing = entity.IsThrowing;
            _isThrowingDisposable = _isThrowing.Subscribe(OnIsThrowingChanged);

            _lineRenderer.positionCount = _precision;
            _lineRenderer.enabled = false;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isThrowingDisposable?.Dispose();
        }

        public void SetHookTransform(Transform hookTransform)
        {
            _hookTransform = hookTransform;
            _animationTime = 0; // Сбрасываем анимацию при новом броске
        }

        public void ClearHookTransform()
        {
            _hookTransform = null;
        }

        private void LateUpdate()
        {
            if (!_lineRenderer.enabled || _hookTransform == null || _ropeOrigin == null)
                return;

            _animationTime += Time.deltaTime * 5f; // Скорость "успокоения" веревки
            DrawRope();
        }

        private void DrawRope()
        {
            Vector3 startPos = _ropeOrigin.position;
            Vector3 endPos = _hookTransform.position;

            for (int i = 0; i < _precision; i++)
            {
                float delta = (float)i / (_precision - 1);

                // Основная линия между точками
                Vector3 pos = Vector3.Lerp(startPos, endPos, delta);

                // Добавляем эффект волны, если веревка еще "свежая"
                if (_animationTime < 1f)
                {
                    float wave = Mathf.Sin(delta * _waveFrequency * Mathf.PI) * _waveAmplitude;
                    // Затухание волны со временем и по краям веревки (чтобы концы были закреплены)
                    float multiplier = Mathf.Sin(delta * Mathf.PI) * (1f - _animationTime);

                    pos += Vector3.up * wave * multiplier;
                }

                _lineRenderer.SetPosition(i, pos);
            }
        }

        private void OnIsThrowingChanged(bool oldValue, bool value)
        {
            _lineRenderer.enabled = value;
            if (value) _animationTime = 0;
        }
    }
}