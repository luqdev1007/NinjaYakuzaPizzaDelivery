using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashVFXView : EntityView
    {
        [Header("Afterimage")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField] private SpriteRenderer _heroSpriteRenderer;
        [SerializeField, Min(0.01f)] private float _spawnInterval = 0.04f;
        [SerializeField, Min(0.1f)] private float _afterimageLifetime = 0.2f;
        [SerializeField] private Color _afterimageColor = new Color(0.5f, 0.7f, 1f, 0.8f);
        [SerializeField] private int _poolSize = 8;

        private IReadOnlyVariable<bool> _isDashing;
        private IDisposable _isDashingDisposable;

        private GameObjectPool _pool;
        private float _spawnTimer;
        private bool _dashing;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDashing = entity.IsDashing;
            _isDashingDisposable = _isDashing.Subscribe(OnIsDashingChanged);

            _pool = new GameObjectPool(_afterimagePrefab, null, _poolSize);
        }

        private void Update()
        {
            if (!_dashing)
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
            if (_heroSpriteRenderer == null || _heroSpriteRenderer.sprite == null)
                return;

            GameObject obj = _pool.Get();

            if (!obj.TryGetComponent<AfterimageInstance>(out var instance))
                return;

            instance.Initialize(
                _heroSpriteRenderer.sprite,
                _heroSpriteRenderer.transform.position,
                _heroSpriteRenderer.transform.lossyScale,
                _afterimageLifetime,
                _afterimageColor,
                ReturnToPool);
        }

        private void ReturnToPool(GameObject obj)
        {
            _pool.Return(obj);
        }

        private void OnIsDashingChanged(bool oldValue, bool value)
        {
            _dashing = value;
            _spawnTimer = 0f;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isDashingDisposable?.Dispose();
        }
    }
}