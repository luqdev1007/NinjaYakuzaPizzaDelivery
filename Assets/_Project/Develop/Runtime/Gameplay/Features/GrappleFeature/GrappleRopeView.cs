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
        [SerializeField] private int _precision = 20;
        [SerializeField] private float _waveAmplitude = 0.5f;
        [SerializeField] private float _waveFrequency = 2f;
        [SerializeField] private float _straightenSpeed = 5f;

        private IReadOnlyVariable<bool> _isThrowing;
        private IDisposable _isThrowingDisposable;

        private Transform _hookTransform;
        private Vector3? _staticTargetPoint; // Для фиксации веревки, если снаряд удален
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
            _staticTargetPoint = null;
            _animationTime = 0;
            _lineRenderer.enabled = true;
        }

        public void FixateToPoint(Vector3 worldPoint)
        {
            _staticTargetPoint = worldPoint;
            _hookTransform = null;
        }

        public void ClearHookTransform()
        {
            _hookTransform = null;
            _staticTargetPoint = null;
            _lineRenderer.enabled = false;
        }

        private void LateUpdate()
        {
            if (!_lineRenderer.enabled || _ropeOrigin == null) return;

            Vector3 targetPos;
            if (_hookTransform != null)
                targetPos = _hookTransform.position;
            else if (_staticTargetPoint.HasValue)
                targetPos = _staticTargetPoint.Value;
            else return;

            _animationTime += Time.deltaTime * _straightenSpeed;
            DrawRope(_ropeOrigin.position, targetPos);
        }

        private void DrawRope(Vector3 startPos, Vector3 endPos)
        {
            for (int i = 0; i < _precision; i++)
            {
                float delta = (float)i / (_precision - 1);
                Vector3 pos = Vector3.Lerp(startPos, endPos, delta);

                if (_animationTime < 1f)
                {
                    float wave = Mathf.Sin(delta * _waveFrequency * Mathf.PI) * _waveAmplitude;
                    float multiplier = Mathf.Sin(delta * Mathf.PI) * (1f - _animationTime);
                    pos += Vector3.up * wave * multiplier;
                }

                _lineRenderer.SetPosition(i, pos);
            }
        }

        private void OnIsThrowingChanged(bool oldValue, bool value)
        {
            if (!value) ClearHookTransform();
        }
    }
}