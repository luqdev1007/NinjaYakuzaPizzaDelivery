using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _isDashingAnimParam = "IsDashing";

        [Header("Flash VFX")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _flashDuration = 0.1f;
        [SerializeField] private Color _flashColor = Color.white;

        [Header("Afterimage VFX")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField] private float _spawnInterval = 0.04f;
        [SerializeField] private float _afterimageLifetime = 0.2f;
        [SerializeField] private Color _afterimageColor = new Color(0.5f, 0.7f, 1f, 0.8f);
        [SerializeField] private int _poolSize = 8;

        [Header("Audio Settings")]
        [SerializeField] private string _dashSoundPrefix = "AbilityImpactCharge";
        [SerializeField] private Vector2 _dashPitchRange = new Vector2(1.2f, 1.4f);
        [SerializeField] private AudioCategoryType _hitCategory = AudioCategoryType.HeroAttackHit;
        [SerializeField] private float _hitPitchBase = 1.4f;

        private AudioService _audioService;
        private IDisposable _dashDisposable;
        private IDisposable _damageEventDisposable;

        private GameObjectPool _pool;
        private float _spawnTimer;
        private bool _isDashingInternal;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private readonly int IsDashingKey = Animator.StringToHash("IsDashing");

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _pool = new GameObjectPool(_afterimagePrefab, null, _poolSize);

            _dashDisposable = entity.IsDashing.Subscribe(OnDashChanged);

            if (entity.HasComponent<TakeDamageRequest>())
            {
                _damageEventDisposable = entity.TakeDamageEvent.Subscribe(_ => PlayHitSound());
            }

            UpdateVisuals(entity.IsDashing.Value);
        }

        private void Update()
        {
            if (!_isDashingInternal) return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnAfterimage();
                _spawnTimer = _spawnInterval;
            }
        }

        private void OnDashChanged(bool old, bool value)
        {
            _isDashingInternal = value;
            _spawnTimer = 0f;
            UpdateVisuals(value);

            if (value)
            {
                float pitch = UnityEngine.Random.Range(_dashPitchRange.x, _dashPitchRange.y);
                _audioService.PlaySfxVariation(_dashSoundPrefix, 1, 5, pitch);

                if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
                _flashCoroutine = StartCoroutine(FlashCoroutine());
            }
        }

        private void PlayHitSound()
        {
            if (!_isDashingInternal) return;
            float pitch = _hitPitchBase + UnityEngine.Random.Range(-0.1f, 0.1f);
            _audioService.PlayRandomSfx(_hitCategory, true, pitch);
        }

        private void UpdateVisuals(bool value)
        {
            if (_animator != null) _animator.SetBool(IsDashingKey, value);
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
            Color originalColor = Color.white;

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

            ResetFlash();
        }

        private void ResetFlash()
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, Color.white);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _dashDisposable?.Dispose();
            _damageEventDisposable?.Dispose();
            ResetFlash();
        }
    }
}