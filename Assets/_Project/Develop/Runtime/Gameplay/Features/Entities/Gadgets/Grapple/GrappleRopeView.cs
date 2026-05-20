using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
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

        private IReadOnlyVariable<bool> _isGrappling;
        private IReadOnlyVariable<Transform> _hookTransform;
        private IReadOnlyVariable<Vector3> _anchorPoint;

        private IDisposable _isGrapplingDisposable;
        private float _animationTime;

        private void OnValidate() => _lineRenderer ??= GetComponent<LineRenderer>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGrappling = entity.IsGrappling;
            _hookTransform = entity.GrappleHookTransform;
            _anchorPoint = entity.GrappleAnchorPoint;

            _isGrapplingDisposable = _isGrappling.Subscribe(OnIsGrapplingChanged);

            _lineRenderer.positionCount = _precision;
            _lineRenderer.enabled = false;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isGrapplingDisposable?.Dispose();
        }

        private void OnIsGrapplingChanged(bool oldValue, bool value)
        {
            _lineRenderer.enabled = value;
            if (value)
            {
                _animationTime = 0f; // Сбрасываем таймер анимации волны при новом выстреле
            }
        }

        private void LateUpdate()
        {
            if (!_lineRenderer.enabled || _ropeOrigin == null || !_isGrappling.Value)
                return;

            Vector3 targetPos;

            // Если крюк ещё летит — привязываемся к его трансформу
            if (_hookTransform.Value != null)
            {
                targetPos = _hookTransform.Value.position;
            }
            // Если прилетел — берём точку зацепа из параметров симуляции
            else
            {
                targetPos = _anchorPoint.Value;
            }

            _animationTime += Time.deltaTime * _straightenSpeed;
            DrawRope(_ropeOrigin.position, targetPos);
        }

        private void DrawRope(Vector3 startPos, Vector3 endPos)
        {
            Vector3 direction = endPos - startPos;
            Vector3 upDir = Vector3.Cross(direction, Vector3.forward).normalized;
            if (upDir == Vector3.zero) upDir = Vector3.up;

            for (int i = 0; i < _precision; i++)
            {
                float delta = (float)i / (_precision - 1);
                Vector3 pos = Vector3.Lerp(startPos, endPos, delta);

                if (_animationTime < 1f)
                {
                    float edgeFade = Mathf.Sin(delta * Mathf.PI);
                    float wave = Mathf.Sin(delta * _waveFrequency * Mathf.PI + Time.time) * _waveAmplitude;
                    float multiplier = edgeFade * Mathf.Pow(1f - _animationTime, 2);
                    pos += upDir * wave * multiplier;
                }

                _lineRenderer.SetPosition(i, pos);
            }
        }
    }
}