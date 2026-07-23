using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.ObjectsManagment;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash
{
    public class DashView : EntityView, IRequireAudioService
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

        [Header("SFX Keys")]
        [SerializeField] private string _dashSfxKey = "DashExecute";

        private bool _isDashing;
        private IDisposable _dashDisposable;
        private IDisposable _intentDashDisposable;

        private GameObjectPool _pool;
        private GameObject _poolRoot;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;

        // Выданные наружу и ещё не догоревшие афтеримиджи. Нужны только затем,
        // чтобы Cleanup мог вернуть их до уничтожения рута.
        private readonly List<GameObject> _liveAfterimages = new();

        private float _vfxSpawnTimer;
        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake() => _propertyBlock = new MaterialPropertyBlock();

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Рут пула. Раньше пул создавался с parent = null, из-за чего его
            // объекты оседали в КОРНЕ СЦЕНЫ и переживали релиз героя: каждая
            // смерть-респавн добавляла в сцену новую партию из _poolSize штук,
            // и никто их не убирал до выгрузки сцены.
            //
            // Рут НЕ парентится к герою: афтеримидж — отпечаток в мире, он обязан
            // остаться там, где снят. Ребёнок героя ехал бы за ним и превращался
            // в приклеенный к спрайту шлейф. Поэтому рут отдельный объект в корне,
            // но теперь у него есть владелец, который его уничтожает.
            _poolRoot = new GameObject($"AfterimagePool (Dash) [{name}]");

            _pool = new GameObjectPool(_afterimagePrefab, _poolRoot.transform, _poolSize);

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

            _audioService?.PlaySfx(_dashSfxKey, transform.position);
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
                _liveAfterimages.Add(obj);

                instance.Initialize(
                    _spriteRenderer.sprite,
                    _spriteRenderer.transform.position,
                    _spriteRenderer.transform.lossyScale,
                    _afterimageLifetime,
                    _afterimageColor,
                    ReturnToPool);

                obj.transform.rotation = _spriteRenderer.transform.rotation;
            }
        }

        // Прослойка вместо прямого _pool.Return: список живых копий обязан
        // вычищаться ровно тогда, когда копия догорела, иначе Cleanup стал бы
        // возвращать в пул то, что там уже лежит.
        private void ReturnToPool(GameObject obj)
        {
            _liveAfterimages.Remove(obj);

            _pool.Return(obj);
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
            _intentDashDisposable?.Dispose();

            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);

            SetSpriteColor(Color.white);

            // Обратный проход: ReturnToPool вычёркивает элемент из этого же списка.
            for (int i = _liveAfterimages.Count - 1; i >= 0; i--)
            {
                GameObject afterimage = _liveAfterimages[i];

                if (afterimage != null)
                {
                    _pool.Return(afterimage);
                }
            }

            _liveAfterimages.Clear();

            // Уничтожение рута забирает с собой весь пул — и свободные объекты,
            // и только что возвращённые.
            if (_poolRoot != null)
            {
                Destroy(_poolRoot);
                _poolRoot = null;
            }

            _pool = null;
        }
    }
}
