using UnityEngine;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaController : MonoBehaviour
    {
        public event Action<float> OnFired;

        [field: SerializeField] public Transform CameraPivot { get; private set; }
        [field: SerializeField] public Transform ProjectileParent { get; private set; }
        [field: SerializeField] public Rigidbody EngineerPivot { get; private set; }

        [SerializeField] private Rigidbody _horizontalPivot;
        [SerializeField] private Rigidbody _verticalPivot;

        [SerializeField] private float _rotationSpeed = 50f;
        [SerializeField] private float _shootPower = 25;
        [SerializeField] private Vector2 _verticalLimits = new Vector2(-20f, 45f);
        [SerializeField] private Vector2 _horizontalLimits = new Vector2(-60f, 60f);

        [Header("Rate of Fire Settings")]
        [SerializeField] private float _fireRate = 1f;

        private IInputService _inputService;
        private float _currentXRotation;
        private float _currentYRotation;
        private float _lastFireTime;

        private Vector3 _recoilPosOffset;
        private float _recoilPitchOffset;

        public bool IsCharging { get; private set; }
        public float ChargeProgress { get; private set; }
        public float FireCycleDuration => 1f / _fireRate;
        public float ShootPower => _shootPower;

        public void Init(IInputService inputService)
        {
            _inputService = inputService;

            Vector3 localH = _horizontalPivot.transform.localEulerAngles;
            Vector3 localV = _verticalPivot.transform.localEulerAngles;

            _currentYRotation = FixAngle(localH.y);
            _currentXRotation = FixAngle(localV.x);
        }

        public void SetRecoilOffsets(Vector3 posOffset, float pitchOffset)
        {
            _recoilPosOffset = posOffset;
            _recoilPitchOffset = pitchOffset;
        }

        private float FixAngle(float angle)
        {
            if (angle > 180) 
                angle -= 360;

            return angle;
        }

        private void Update()
        {
            if (_inputService == null || !_inputService.IsEnabled)
                return;

            HandleCharge();
        }

        private void FixedUpdate()
        {
            if (_inputService == null || !_inputService.IsEnabled) 
                return;

            HandlePhysicsRotation();
        }

        private void HandlePhysicsRotation()
        {
            Vector2 input = _inputService.MoveDirection;

            _currentYRotation += input.x * _rotationSpeed * Time.fixedDeltaTime;
            _currentYRotation = Mathf.Clamp(_currentYRotation, _horizontalLimits.x, _horizontalLimits.y);

            _currentXRotation += input.y * _rotationSpeed * Time.fixedDeltaTime;
            _currentXRotation = Mathf.Clamp(_currentXRotation, _verticalLimits.x, _verticalLimits.y);

            Quaternion rootRotation = transform.rotation;

            // Сдвигаем всё основание баллисты (прыжок и откат)
            _horizontalPivot.MovePosition(transform.position + rootRotation * _recoilPosOffset);

            // Поворачиваем горизонталь
            Quaternion horizTarget = rootRotation * Quaternion.Euler(0, _currentYRotation, 0);
            _horizontalPivot.MoveRotation(horizTarget);

            // Поворачиваем вертикаль + добавляем задирание носа
            Quaternion vertTarget = rootRotation * Quaternion.Euler(_currentXRotation + _recoilPitchOffset, _currentYRotation, 0);
            _verticalPivot.MoveRotation(vertTarget);
        }

        private void HandleCharge()
        {
            if (Time.time < _lastFireTime + FireCycleDuration)
            {
                IsCharging = false;
                return;
            }

            IsCharging = _inputService.IsAttackKeyHold;

            if (IsCharging) 
                ChargeProgress = Mathf.Clamp01(ChargeProgress + Time.deltaTime / 2f);

            if (_inputService.IsAttackKeyReleased && ChargeProgress > 0)
            {
                _lastFireTime = Time.time;
                ChargeProgress = 0;

                OnFired?.Invoke(ChargeProgress);
            }
        }
    }
}