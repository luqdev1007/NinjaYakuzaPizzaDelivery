using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView
    {
        private readonly int IsAttackingKey = Animator.StringToHash("IsAttacking");

        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _slashParticle;

        private ParticleSystemRenderer _particleRenderer;
        private Transform _rootTransform; // Ссылка на ту самую верхнюю пустышку
        private IReadOnlyVariable<bool> _inAttackProcess;
        private IDisposable _inAttackProcessChangedDisposable;
        private IDisposable _attackHitDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _inAttackProcess = entity.InAttackProcess;

            // Берем ссылку на трансформ главной пустышки из сущности
            _rootTransform = entity.Transform;

            if (_slashParticle != null)
                _particleRenderer = _slashParticle.GetComponent<ParticleSystemRenderer>();

            _inAttackProcessChangedDisposable = _inAttackProcess.Subscribe(OninAttackProcessChanged);
            UpdateInAttackProcess(_inAttackProcess.Value);

            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(PlaySlashEffect);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _inAttackProcessChangedDisposable?.Dispose();
            _attackHitDisposable?.Dispose();
        }

        private void PlaySlashEffect()
        {
            if (_slashParticle == null || _particleRenderer == null || _rootTransform == null)
                return;

            // Читаем скейл САМОЙ ВЕРХНЕЙ пустышки, которую крутит логика
            float rootScaleX = _rootTransform.localScale.x;

            // Твоя логика: вправо (scale > 0) -> flip 1, влево -> flip 0
            Vector3 currentFlip = _particleRenderer.flip;
            currentFlip.x = rootScaleX > 0 ? 1 : 0;
            _particleRenderer.flip = currentFlip;

            _slashParticle.Stop();
            _slashParticle.Play();
        }

        private void UpdateInAttackProcess(bool value) => _animator.SetBool(IsAttackingKey, value);
        private void OninAttackProcessChanged(bool oldVal, bool newVal) => UpdateInAttackProcess(newVal);
    }
}