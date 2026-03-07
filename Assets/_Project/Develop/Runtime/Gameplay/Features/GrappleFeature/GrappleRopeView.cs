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
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _ropeOrigin;

        private IReadOnlyVariable<bool> _isThrowingHook;
        private IReadOnlyVariable<Vector3> _anchorPoint;
        private IDisposable _isThrowingHookDisposable;

        private Transform _hookTransform;

        private void OnValidate()
        {
            _lineRenderer ??= GetComponent<LineRenderer>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isThrowingHook = entity.IsThrowingHook;
            _anchorPoint = entity.GrappleAnchorPoint;
            _isThrowingHookDisposable = _isThrowingHook.Subscribe(OnIsThrowingHookChanged);

            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isThrowingHookDisposable?.Dispose();
        }

        public void SetHookTransform(Transform hookTransform)
        {
            _hookTransform = hookTransform;
        }

        public void ClearHookTransform()
        {
            _hookTransform = null;
        }

        private void LateUpdate()
        {
            if (!_lineRenderer.enabled)
                return;

            Vector3 endPoint = _hookTransform != null
                ? _hookTransform.position
                : _anchorPoint.Value;

            _lineRenderer.SetPosition(0, _ropeOrigin.position);
            _lineRenderer.SetPosition(1, endPoint);
        }

        private void OnIsThrowingHookChanged(bool oldValue, bool value)
        {
            _lineRenderer.enabled = value;
        }
    }
}