using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Визуальная рандомизация призрака на экземпляр: размер, базовая прозрачность,
    // тинт, темп idle-анимации. Образец — AnimatorRandomOffsetView: весь рандом
    // тянется один раз в OnEntityStartedWork.
    //
    // ИСТОЧНИК РАНДОМА — ГЛОБАЛЬНЫЙ UnityEngine.Random, и это сознательно. Всё, что
    // здесь варьируется, не влияет ни на хитбокс, ни на физику, ни на решения AI,
    // поэтому детерминизм не нужен. Геймплейная ветка ушла на засеянный
    // IGameplayRandom, так что глобальный поток больше не течёт в реплей.
    //
    // ПОРЯДОК КОМПОНЕНТОВ КРИТИЧЕН. Этот компонент обязан стоять на объекте View
    // ВЫШЕ ApplyDamageView. MonoEntity.Link подписывает вьюхи на entity.Initialized
    // в порядке GetComponentsInChildren, то есть в порядке компонентов на объекте,
    // а Initialized — обычный C#-event и зовёт обработчики в порядке подписки.
    // ApplyDamageView снимает _originalColor в СВОЁМ OnEntityStartedWork и потом
    // восстанавливает его после каждой damage-вспышки. Если покрасить рендерер
    // позже него, тинт и базовая альфа не попадут в снимок и будут стёрты первой же
    // вспышкой. Стоя выше, мы красим раньше — снимок берётся уже с вариацией.
    public class GhostVisualVariationView : EntityView
    {
        [Header("Refs")]
        [Tooltip("ViewContainer — родитель View. Скейл вешается СЮДА, а не на View и не на корень: " +
                 "на корне живут CapsuleCollider (хитбокс) и Jumpable, а View.localScale " +
                 "абсолютной кривой перетирает GhostDeathAnimation.")]
        [SerializeField] private Transform _viewContainer;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;

        [Header("Size")]
        [SerializeField] private float _minScale = 0.85f;
        [SerializeField] private float _maxScale = 1.15f;

        [Header("Base Alpha")]
        [Tooltip("Базовая прозрачность на экземпляр. Глитч отсчитывает вспышки именно от неё — " +
                 "GhostGlitchView снимает базу с рендерера уже после этого компонента.")]
        [SerializeField] private float _minBaseAlpha = 0.4f;
        [SerializeField] private float _maxBaseAlpha = 0.6f;

        [Header("Tint")]
        [Tooltip("Насколько сильно канал может уйти от исходного цвета. Держать малым — " +
                 "тинт должен читаться как оттенок, а не как перекраска")]
        [SerializeField] private float _tintStrength = 0.12f;

        [Header("Idle Animation Speed")]
        [Tooltip("Множитель темпа idle-анимации. Писателей Animator.speed у призрака нет, поле свободно. " +
                 "С рандом-оффсетом фазы из AnimatorRandomOffsetView не конфликтует: там фаза, здесь темп")]
        [SerializeField] private float _minAnimatorSpeed = 0.9f;
        [SerializeField] private float _maxAnimatorSpeed = 1.1f;

        protected override void OnEntityStartedWork(Entity entity)
        {
            ApplySize();
            ApplyColor();
            ApplyAnimatorSpeed();
        }

        private void ApplySize()
        {
            if (_viewContainer == null)
            {
                return;
            }

            // Равномерный скейл. ViewContainer свободен: ни один клип его не адресует
            // (у всех кривых пустой path, то есть цель — объект с Animator, а он на View),
            // коллайдеров под ним нет. Death-клип продолжает работать: он гонит
            // View.localScale, а ViewContainer домножает, поэтому призрак схлопывается
            // от своего размера, а не от единицы.
            float scale = Random.Range(_minScale, _maxScale);
            _viewContainer.localScale = Vector3.one * scale;
        }

        private void ApplyColor()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            Color color = _spriteRenderer.color;

            // Тинт: каждый канал независимо чуть уводится вниз от исходного, что даёт
            // лёгкий случайный оттенок вместо перекраски. Пишем цвет и альфу одной
            // записью — до того, как ApplyDamageView снимет свой _originalColor.
            color.r *= 1f - Random.Range(0f, _tintStrength);
            color.g *= 1f - Random.Range(0f, _tintStrength);
            color.b *= 1f - Random.Range(0f, _tintStrength);

            color.a = Random.Range(_minBaseAlpha, _maxBaseAlpha);

            _spriteRenderer.color = color;
        }

        private void ApplyAnimatorSpeed()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.speed = Random.Range(_minAnimatorSpeed, _maxAnimatorSpeed);
        }
    }
}
