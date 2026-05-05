using System;
using System.Collections.Generic;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class PizzaHealthView : EntityView
    {
        [Header("Pizza Visuals (In Hierarchy)")]
        [SerializeField] private List<GameObject> _pizzaSlices;
        [SerializeField] private ParticleSystem _cheeseDripEffect;
        [SerializeField] private Rigidbody2D _pizzaRigidbody;

        [Header("Debris Settings (From Pool)")]
        [SerializeField] private GameObject _sliceDebrisPrefab;
        [SerializeField] private float _minLaunchForce = 3f;
        [SerializeField] private float _maxLaunchForce = 5f;

        [Header("Shake Settings")]
        [SerializeField] private Transform _visualContainer;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 0.5f;

        [Header("UI References")]
        [SerializeField] private LivesCountView _livesUI;

        private GameObjectPool _debrisPool;
        private IDisposable _healthDisposable;
        private Rigidbody2D _heroRigidbody;
        private int _currentVisibleSlices;
        private float _maxHealthCached;

        protected override void OnDependencyResolve(DIContainer container)
        {
            _debrisPool = new GameObjectPool(_sliceDebrisPrefab, null, 5);
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _heroRigidbody = entity.Rigidbody;
            _maxHealthCached = entity.MaxHealth.Value;

            _currentVisibleSlices = CalculateTargetSlices(entity.CurrentHealth.Value);
            RefreshSlicesVisibility();

            _healthDisposable = entity.CurrentHealth.Subscribe(OnHealthChanged);
        }

        private void FixedUpdate()
        {
            if (_heroRigidbody == null || _pizzaRigidbody == null)
                return;

            float heroVelocityX = _heroRigidbody.linearVelocity.x;

            if (Mathf.Abs(heroVelocityX) > 0.1f)
            {
                _pizzaRigidbody.AddTorque(-heroVelocityX * 0.2f, ForceMode2D.Force);
            }
        }

        private void OnHealthChanged(float oldHp, float newHp)
        {
            if (newHp > oldHp)
            {
                _currentVisibleSlices = CalculateTargetSlices(newHp);
                RefreshSlicesVisibility();
                _livesUI?.Show(_currentVisibleSlices);
                return;
            }

            int targetSlices = CalculateTargetSlices(newHp);

            if (targetSlices < _currentVisibleSlices)
            {
                int slicesToRemove = _currentVisibleSlices - targetSlices;
                for (int i = 0; i < slicesToRemove; i++)
                {
                    EjectSliceDebris();
                }

                _currentVisibleSlices = targetSlices;
                RefreshSlicesVisibility();

                _cheeseDripEffect?.Play();
                ShakePizza();
                _livesUI?.Show(_currentVisibleSlices);
            }
        }

        private int CalculateTargetSlices(float health)
        {
            float healthPercent = health / _maxHealthCached;
            return Mathf.CeilToInt(healthPercent * _pizzaSlices.Count);
        }

        private void RefreshSlicesVisibility()
        {
            for (int i = 0; i < _pizzaSlices.Count; i++)
            {
                _pizzaSlices[i].SetActive(i < _currentVisibleSlices);
            }
        }

        private void EjectSliceDebris()
        {
            if (_debrisPool == null) return;

            GameObject debris = _debrisPool.Get();
            debris.transform.position = transform.position;
            debris.transform.rotation = transform.rotation;

            if (debris.TryGetComponent<Rigidbody2D>(out var rb))
            {
                float force = Random.Range(_minLaunchForce, _maxLaunchForce);
                Vector2 direction = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
                rb.linearVelocity = direction * force;
                rb.angularVelocity = Random.Range(-360f, 360f);
            }
        }

        private void ShakePizza()
        {
            if (_visualContainer != null)
            {
                _visualContainer.DOComplete();
                _visualContainer.DOShakePosition(_shakeDuration, _shakeStrength);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _healthDisposable?.Dispose();
            _visualContainer?.DOKill();
        }
    }
}