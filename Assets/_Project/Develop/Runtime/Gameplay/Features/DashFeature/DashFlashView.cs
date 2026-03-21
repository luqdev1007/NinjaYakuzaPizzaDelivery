using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashFlashView : EntityView
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField, Min(0f)] private float _flashDuration = 0.1f;
        [SerializeField] private Color _flashColor = Color.white;

        private IReadOnlyVariable<bool> _isDashing;
        private IDisposable _isDashingDisposable;
        private MaterialPropertyBlock _propertyBlock;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDashing = entity.IsDashing;
            _isDashingDisposable = _isDashing.Subscribe(OnIsDashingChanged);
        }

        private void OnIsDashingChanged(bool oldValue, bool value)
        {
            if (value)
            {
                if (_flashCoroutine != null)
                    StopCoroutine(_flashCoroutine);
                _flashCoroutine = StartCoroutine(FlashCoroutine());
            }
        }

        private IEnumerator FlashCoroutine()
        {
            float elapsed = 0f;
            Color originalColor = _spriteRenderer.color;

            while (elapsed < _flashDuration)
            {
                float t = elapsed / _flashDuration;
                Color current = Color.Lerp(_flashColor, originalColor, t);

                _spriteRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(ColorProperty, current);
                _spriteRenderer.SetPropertyBlock(_propertyBlock);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, originalColor);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDashingDisposable?.Dispose();

            if (_propertyBlock != null)
            {
                _spriteRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(ColorProperty, Color.white);
                _spriteRenderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}