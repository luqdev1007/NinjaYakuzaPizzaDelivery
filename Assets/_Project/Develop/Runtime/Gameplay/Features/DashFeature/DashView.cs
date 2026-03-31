using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashView : EntityView
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Audio")]
        [SerializeField] private string _dashSoundPrefix = "AbilityImpactCharge";
        [SerializeField] private string _hitPrefix = "EnemyHit";

        [Header("Afterimage (VFX)")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField, Min(0.01f)] private float _spawnInterval = 0.04f;
        [SerializeField, Min(0.1f)] private float _afterimageLifetime = 0.2f;
        [SerializeField] private Color _afterimageColor = new Color(0.5f, 0.7f, 1f, 0.8f);
        [SerializeField] private int _poolSize = 8;

        [Header("Flash (VFX)")]
        [SerializeField, Min(0f)] private float _flashDuration = 0.1f;
        [SerializeField] private Color _flashColor = Color.white;

        private AudioService _audioService;
        private GameObjectPool _pool;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;

        private IDisposable _dashDisposable;
        private IDisposable _damageEventDisposable;

        private float _spawnTimer;
        private bool _isDashing;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int IsDashingKey = Animator.StringToHash("IsDashing");

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _pool = new GameObjectPool(_afterimagePrefab, null, _poolSize);

            // Основная подписка на рывок
            _dashDisposable = entity.IsDashing.Subscribe((old, value) =>
            {
                _isDashing = value;
                _spawnTimer = 0f;

                if (value)
                {
                    // Звук
                    _audioService.PlaySfxByPrefixAuto(_dashSoundPrefix, UnityEngine.Random.Range(1.2f, 1.4f));

                    // Анимация
                    if (_animator) _animator.SetBool(IsDashingKey, true);

                    // Вспышка
                    if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
                    _flashCoroutine = StartCoroutine(FlashCoroutine());
                }
                else
                {
                    if (_animator) _animator.SetBool(IsDashingKey, false);
                }
            });

            // Звук удара во время рывка
            if (entity.HasComponent<TakeDamageRequest>())
            {
                _damageEventDisposable = entity.TakeDamageEvent.Subscribe(_ =>
                {
                    if (_isDashing)
                        _audioService.PlaySfxByPrefixAuto(_hitPrefix, 1.4f);
                });
            }
        }

        private void Update()
        {
            if (!_isDashing) return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnAfterimage();
                _spawnTimer = _spawnInterval;
            }
        }

        private void SpawnAfterimage()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null) return;

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
            Color originalColor = Color.white; // Обычно спрайт по умолчанию белый под модификатором

            while (elapsed < _flashDuration)
            {
                float t = elapsed / _flashDuration;
                Color current = Color.Lerp(_flashColor, originalColor, t);

                SetSpriteColor(current);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetSpriteColor(originalColor);
        }

        private void SetSpriteColor(Color color)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, color);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _dashDisposable?.Dispose();
            _damageEventDisposable?.Dispose();

            if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
            SetSpriteColor(Color.white);
        }
    }
}