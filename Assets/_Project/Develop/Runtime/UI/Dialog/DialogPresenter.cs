using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.UI.TextFeatures; // Подключили утилиту
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogPresenter : PopupPresenterBase
    {
        public event Action DialogEnded;

        private readonly DialogDisplayView _view;
        private readonly DialogConfig _config;
        private readonly CharactersConfig _charactersConfig;

        private int _currentLineIndex = -1;
        private bool _isTyping;
        private float _currentHoldTime;
        private bool _isHolding;

        // Таймер для быстрого пролистывания при зажатии
        private float _fastForwardTimer;

        private const KeyCode SkipKey = KeyCode.E;
        private const float SkipHoldDuration = 0.8f; // Уменьшил, чтобы быстрее реагировало
        private const float FastForwardInterval = 0.2f; // Скорость прокрутки при зажатии

        public DialogPresenter(
            DialogDisplayView view,
            ICoroutinesPerformer coroutinesPerformer,
            DialogConfig config,
            CharactersConfig charactersConfig) : base(coroutinesPerformer)
        {
            _view = view;
            _config = config;
            _charactersConfig = charactersConfig;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();
            _view.AppearanceFinished += OnAppearanceFinished;
            _view.ShowSkipHint();
        }

        private void OnAppearanceFinished()
        {
            _view.AppearanceFinished -= OnAppearanceFinished;
            ShowNextLine();
        }

        public void Update(float deltaTime)
        {
            HandleProgressInput();
            HandleSkipInput(deltaTime);
        }

        private void HandleProgressInput()
        {
            // Теперь и E (нажатие) тоже продвигает диалог
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(SkipKey))
            {
                ProgressDialog();
            }
        }

        private void ProgressDialog()
        {
            if (_isTyping)
            {
                FinishTyping();
                _view.FinishTypingInstant(); // Нужно, чтобы View сразу показала весь текст
            }
            else
            {
                ShowNextLine();
            }
        }

        private void HandleSkipInput(float deltaTime)
        {
            if (Input.GetKeyDown(SkipKey))
            {
                _isHolding = true;
                _currentHoldTime = 0f;
                _fastForwardTimer = 0f;
                _view.StartHoldAnims(SkipHoldDuration);
            }

            if (Input.GetKey(SkipKey) && _isHolding)
            {
                _currentHoldTime += deltaTime;
                _fastForwardTimer += deltaTime;

                // Если зажали — начинаем быстро пролистывать реплики
                if (_fastForwardTimer >= FastForwardInterval)
                {
                    _fastForwardTimer = 0f;
                    ProgressDialog();
                }

                // Если держим долго — полный выход из диалога (взрыв)
                if (_currentHoldTime >= SkipHoldDuration)
                {
                    _isHolding = false;
                    _view.ExplodeSkip();
                    EndDialog();
                }
            }

            if (Input.GetKeyUp(SkipKey))
            {
                _isHolding = false;
                _view.StopHoldAnims();
            }
        }

        private void ShowNextLine()
        {
            _currentLineIndex++;

            if (_currentLineIndex >= _config.Replicas.Count)
            {
                EndDialog();
                return;
            }

            DialogReplica replica = _config.Replicas[_currentLineIndex];
            CharacterData characterData = _charactersConfig.GetCharacter(replica.CharacterId);

            // Обработка текста утилитой подсветки перед выводом
            string processedText = TextHighlightUtility.ProcessText(replica.RawText);

            _view.SetText(processedText);
            _view.SetPortrait(characterData.Portrait);
            _view.SetBackground(characterData.Background);

            _isTyping = true; // Считаем, что начали печатать
        }

        private void FinishTyping()
        {
            _isTyping = false;
        }

        private void EndDialog()
        {
            DialogEnded?.Invoke();
            OnCloseRequest();
        }

        public override void Dispose()
        {
            _view.AppearanceFinished -= OnAppearanceFinished;
            base.Dispose();
        }
    }
}