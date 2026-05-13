using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.ObjectsManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashView : EntityView
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int IsDashingKey = Animator.StringToHash("IsDashing");

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Afterimage (VFX)")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField, Min(0.01f)] private float _spawnInterval = 0.04f;
        [SerializeField, Min(0.1f)] private float _afterimageLifetime = 0.2f;
        [SerializeField] private Color _afterimageColor = new Color(0.5f, 0.7f, 1f, 0.8f);
        [SerializeField] private int _poolSize = 8;

        [Header("Flash (VFX)")]
        [SerializeField, Min(0f)] private float _flashDuration = 0.1f;
        [SerializeField] private Color _flashColor = Color.white;


        private bool _isDashing;
        private IDisposable _dashDisposable;

        private GameObjectPool _pool;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;

        private float _vfxSpawnTimer;

        private void Awake() => _propertyBlock = new MaterialPropertyBlock();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _pool = new GameObjectPool(_afterimagePrefab, null, _poolSize);

            _dashDisposable = entity.IsDashing.Subscribe((oldValue, newValue) =>
            {
                _isDashing = newValue;

                if (newValue) 
                    HandleDashStart();
                else 
                    HandleDashEnd();
            });
        }

        private void HandleDashStart()
        {
            _vfxSpawnTimer = 0f;

            _animator.SetBool(IsDashingKey, true);

            if (_flashCoroutine != null) 
                StopCoroutine(_flashCoroutine);

            _flashCoroutine = StartCoroutine(FlashCoroutine());
        }

        private void HandleDashEnd()
        {
            _animator.SetBool(IsDashingKey, false);
        }

        private void Update()
        {
            if (!_isDashing) 
                return;

            HandleVFX();
        }

        private void HandleVFX()
        {
            _vfxSpawnTimer -= Time.deltaTime;

            if (_vfxSpawnTimer <= 0f)
            {
                SpawnAfterimage();
                _vfxSpawnTimer = _spawnInterval;
            }
        }

        private void SpawnAfterimage()
        {
            GameObject obj = _pool.Get();

            if (obj.TryGetComponent<AfterimageInstance>(out var instance))
            {
                instance.Initialize(
                    _spriteRenderer.sprite,
                    _spriteRenderer.transform.position,
                    _spriteRenderer.transform.lossyScale,
                    _afterimageLifetime,
                    _afterimageColor,
                    _pool.Return);
            }
        }

        private IEnumerator FlashCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < _flashDuration)
            {
                float t = elapsed / _flashDuration;
                SetSpriteColor(Color.Lerp(_flashColor, Color.white, t));
                elapsed += Time.deltaTime;

                yield return null;
            }

            SetSpriteColor(Color.white);
        }

        private void SetSpriteColor(Color color)
        {
            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, color);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _dashDisposable?.Dispose();

            if (_flashCoroutine != null) 
                StopCoroutine(_flashCoroutine);

            SetSpriteColor(Color.white);
        }
    }
}