using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.UI.TextFeatures;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using DG.Tweening;
using UnityEngine;

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
        private bool _isEnding;

        private const float SkipHoldDuration = 0.6f;

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
            _currentLineIndex = -1;
            _isEnding = false;
            _isHolding = false;

            _view.PlayAppearance().OnComplete(() =>
            {
                if (_isEnding) return;
                ShowNextLine();
            });
        }

        public void Update(float deltaTime)
        {
            if (_isEnding || _view == null) return;

            _view.SkipLabel.UpdateIdle(deltaTime);
            HandleInput(deltaTime);
        }

        private void HandleInput(float deltaTime)
        {
            if (_inputService.IsInteractKeyPressed)
            {
                _isHolding = true;
                _currentHoldTime = 0f;
                _view.SkipLabel.StartHoldProgress(SkipHoldDuration);
            }

            if (_isHolding && _inputService.IsInteractKeyHeld)
            {
                _currentHoldTime += deltaTime;
                if (_currentHoldTime >= SkipHoldDuration)
                {
                    _isHolding = false;
                    _view.SkipLabel.StopHoldProgress();
                    EndDialog();
                    return;
                }
            }

            if (_inputService.IsInteractKeyReleased)
            {
                if (_isHolding && _currentHoldTime < SkipHoldDuration)
                {
                    ProgressDialog();
                }
                _isHolding = false;
                _view.SkipLabel.StopHoldProgress();
            }
        }

        private void ProgressDialog()
        {
            if (_isTyping)
            {
                _isTyping = false;
                _view.FinishTypingInstant();
            }
            else
            {
                ShowNextLine();
            }
        }

        private void ShowNextLine()
        {
            if (_isEnding) return;

            _currentLineIndex++;

            if (_currentLineIndex >= _config.Replicas.Count)
            {
                EndDialog();
                return;
            }

            DialogReplica replica = _config.Replicas[_currentLineIndex];
            CharacterData characterData = _charactersConfig.GetCharacter(replica.CharacterId);

            if (characterData != null)
            {
                _view.SetPortrait(characterData.Portrait);
                _view.SetBackground(characterData.Background);
            }

            string processedText = TextHighlightUtility.ProcessText(replica.RawText);

            _isTyping = true;
            _view.SetText(processedText, () => _isTyping = false);
        }

        private void EndDialog()
        {
            if (_isEnding) return;
            _isEnding = true;
            _isHolding = false;

            _view.FinishTypingInstant();
            _view.ClearText();

            _view.PlayDisappearance().OnComplete(() =>
            {
                if (_view == null) return;
                DialogEnded?.Invoke();
                OnCloseRequest();
            });
        }
    }
}