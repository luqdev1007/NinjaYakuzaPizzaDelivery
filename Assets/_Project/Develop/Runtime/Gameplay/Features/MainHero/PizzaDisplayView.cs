using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class PizzaDisplayView : EntityView
    {
        [SerializeField] private List<GameObject> _pizzaSlices;
        [SerializeField] private ParticleSystem _cheeseDripEffect;
        [SerializeField] private Rigidbody2D _rigidbody;

        private int _currentVisibleSlices;
        private Entity _linkedEntity;


        private void FixedUpdate()
        {
            if (_linkedEntity == null)
                return;

            float heroVelocityX = _linkedEntity.Rigidbody.linearVelocity.x;

            if (Mathf.Abs(heroVelocityX) > 0.1f)
            {
                _rigidbody.AddTorque(-heroVelocityX * 0.2f, ForceMode2D.Force);
            }
        }

        public void UpdateHealthVisual(float currentHealth, float maxHealth)
        {
            float healthPercent = currentHealth / maxHealth;
            int targetSlices = Mathf.CeilToInt(healthPercent * _pizzaSlices.Count);

            if (targetSlices < _currentVisibleSlices)
            {
                RemoveSlice();
            }

            _currentVisibleSlices = targetSlices;
        }

        private void RemoveSlice()
        {
            if (_currentVisibleSlices <= 0) 
                return;

            GameObject slice = _pizzaSlices[_currentVisibleSlices - 1];

            slice.transform.SetParent(null);

            var rb = slice.AddComponent<Rigidbody2D>();

            rb.linearVelocity = new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 5f));
            rb.angularVelocity = Random.Range(-360f, 360f);

            slice.transform.DOScale(0, 2f).OnComplete(() => Destroy(slice));

            _cheeseDripEffect.Play();
        }

        internal void Initialize(Entity entity)
        {
            _currentVisibleSlices = _pizzaSlices.Count;
            _linkedEntity = entity;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _linkedEntity = entity;
        }
    }
}