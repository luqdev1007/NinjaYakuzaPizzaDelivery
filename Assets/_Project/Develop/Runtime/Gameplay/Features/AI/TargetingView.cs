using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class TargetingView : EntityView, IRequireAudioService
    {
        [Header("Visuals")]
        [SerializeField] private GameObject _targetMarkerPrefab;
        [SerializeField, Tooltip("Смещение маркера относительно центра цели")]
        private Vector3 _markerOffset = new Vector3(0f, 2f, 0f);

        [Header("SFX Keys")]
        [SerializeField] private string _activationSfxKey = "TargetingActivate";
        [SerializeField] private string _deactivationSfxKey = "TargetingDeactivate";
        [SerializeField] private string _switchTargetSfxKey = "TargetingSwitch";

        private IAudioService _audioService;
        private GameObject _markerInstance;
        private Transform _currentTargetTransform;
        private bool _isSystemActive; // Кешируем состояние активности системы

        private IDisposable _targetingActiveDisposable;
        private IDisposable _currentTargetDisposable;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (_targetMarkerPrefab != null)
            {
                _markerInstance = Instantiate(_targetMarkerPrefab);
                _markerInstance.SetActive(false);
            }

            _targetingActiveDisposable = entity.IsTargetingActive.Subscribe(OnTargetingActiveChanged);
            _currentTargetDisposable = entity.CurrentTarget.Subscribe(OnTargetChanged);
        }

        private void OnTargetingActiveChanged(bool old, bool current)
        {
            _isSystemActive = current;

            if (current)
            {
                _audioService?.PlaySfx(_activationSfxKey, transform.position);

                // Если при активации цель уже каким-то чудом есть — показываем маркер
                if (_currentTargetTransform != null && _markerInstance != null)
                {
                    _markerInstance.SetActive(true);
                }
            }
            else
            {
                _audioService?.PlaySfx(_deactivationSfxKey, transform.position);

                if (_markerInstance != null)
                    _markerInstance.SetActive(false);

                _currentTargetTransform = null;
            }
        }

        private void OnTargetChanged(Entity oldTarget, Entity newTarget)
        {
            if (oldTarget != null && newTarget != null && oldTarget != newTarget)
            {
                _audioService?.PlaySfx(_switchTargetSfxKey, transform.position);
            }

            if (newTarget != null && newTarget.Transform != null)
            {
                _currentTargetTransform = newTarget.Transform;

                // ФИКС: Если система активна, то при появлении новой цели ВСЕГДА включаем маркер
                if (_isSystemActive && _markerInstance != null)
                {
                    _markerInstance.SetActive(true);
                }
            }
            else
            {
                _currentTargetTransform = null;

                // ФИКС: Если цель пропала (умерла/нет никого), просто тушим маркер, не трогая саму систему
                if (_markerInstance != null)
                {
                    _markerInstance.SetActive(false);
                }
            }
        }

        private void LateUpdate()
        {
            // Теперь LateUpdate занимается ТОЛЬКО позиционированием и никого сам не выключает
            if (_markerInstance != null && _markerInstance.activeSelf && _currentTargetTransform != null)
            {
                _markerInstance.transform.position = _currentTargetTransform.position + _markerOffset;
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _targetingActiveDisposable?.Dispose();
            _currentTargetDisposable?.Dispose();

            if (_markerInstance != null)
            {
                Destroy(_markerInstance);
            }
        }
    }
}