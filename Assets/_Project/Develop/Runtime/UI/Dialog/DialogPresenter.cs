using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.UI.TextFeatures;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogPresenter : PopupPresenterBase
    {
        public event Action DialogEnded;

        private readonly DialogDisplayView _view;
        private readonly DialogConfig _config;
        private readonly CharactersConfig _charactersConfig;
        private readonly IInputService _inputService;

        private int _currentLineIndex = -1;
        private bool _isTyping;
        private float _currentHoldTime;
        private bool _isHolding;
        private float _fastForwardTimer;

        private const float SkipHoldDuration = 0.8f;
        private const float FastForwardInterval = 0.2f;

        public DialogPresenter(
            DialogDisplayView view,
            ICoroutinesPerformer coroutinesPerformer,
            DialogConfig config,
            CharactersConfig charactersConfig,
            IInputService inputService) : base(coroutinesPerformer)
        {
            _view = view;
            _config = config;
            _charactersConfig = charactersConfig;
            _inputService = inputService;
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
            if (_inputService.IsInteractKeyPressed)
            {
                ProgressDialog();
            }
        }

        private void ProgressDialog()
        {
            if (_isTyping == true)
            {
                FinishTyping();
                _view.FinishTypingInstant();
            }
            else
            {
                ShowNextLine();
            }
        }

        private void HandleSkipInput(float deltaTime)
        {
            if (_inputService.IsInteractKeyPressed == true)
            {
                _isHolding = true;
                _currentHoldTime = 0f;
                _fastForwardTimer = 0f;
                _view.StartHoldAnims(SkipHoldDuration);
            }

            if (_inputService.IsInteractKeyHeld == true && _isHolding == true)
            {
                _currentHoldTime += deltaTime;
                _fastForwardTimer += deltaTime;

                if (_fastForwardTimer >= FastForwardInterval)
                {
                    _fastForwardTimer = 0f;
                    ProgressDialog();
                }

                if (_currentHoldTime >= SkipHoldDuration)
                {
                    _isHolding = false;
                    _view.ExplodeSkip();
                    EndDialog();
                }
            }

            if (_inputService.IsInteractKeyReleased == true)
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

            string processedText = TextHighlightUtility.ProcessText(replica.RawText);

            _view.SetText(processedText);
            _view.SetPortrait(characterData.Portrait);
            _view.SetBackground(characterData.Background);

            _isTyping = true;
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