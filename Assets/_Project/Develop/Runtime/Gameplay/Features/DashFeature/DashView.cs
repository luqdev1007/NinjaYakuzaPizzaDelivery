using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using Assets._Project.Develop.Infrastructure.DI;
using System;
using UnityEngine;
using DG.Tweening;

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
        [SerializeField] private AfterimageInstance _afterimagePrefab; 
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

        private IDisposable _dashSub;
        private IDisposable _damageSub;

        private float _spawnTimer;
        private bool _isDashing;
        private Color _originalColor = Color.white;

        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int IsDashingKey = Animator.StringToHash("IsDashing");

        private void Awake() => _propertyBlock = new MaterialPropertyBlock();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _pool = new GameObjectPool(_afterimagePrefab.gameObject, null, _poolSize);

            _dashSub = entity.IsDashing.Subscribe(OnDashChanged);

            _damageSub = entity.TakeDamageEvent.Subscribe(_ =>
            {
                if (_isDashing && _audioService != null)
                    _audioService.PlaySfxByPrefixAuto(_hitPrefix, 1.4f);
            });
        }

        private void OnDashChanged(bool old, bool current)
        {
            _isDashing = current;
            _spawnTimer = 0f;

            if (_animator) _animator.SetBool(IsDashingKey, current);

            if (current)
            {
                _audioService?.PlaySfxByPrefixAuto(_dashSoundPrefix, UnityEngine.Random.Range(1.2f, 1.4f));
                PlayDashFlash();
            }
        }

        private void Update()
        {
            if (!_isDashing)
                return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnAfterimage();
                _spawnTimer = _spawnInterval;
            }
        }

        private void SpawnAfterimage()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null) 
                return;

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

        private void PlayDashFlash()
        {
            if (_spriteRenderer == null) 
                return;

            _spriteRenderer.DOKill();

            float t = 0;
            DOTween.To(() => t, x => t = x, 1f, _flashDuration)
                .OnUpdate(() =>
                {
                    Color current = Color.Lerp(_flashColor, _originalColor, t);
                    SetSpriteColor(current);
                })
                .OnComplete(() => SetSpriteColor(_originalColor));
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
            _dashSub?.Dispose();
            _damageSub?.Dispose();
            _spriteRenderer?.DOKill();

            SetSpriteColor(_originalColor);
        }
    }
}