using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
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

        private const KeyCode SkipKey = KeyCode.E;
        private const float SkipHoldDuration = 1.2f;

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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (_isTyping)
                {
                    FinishTyping();
                }
                else
                {
                    ShowNextLine();
                }
            }
        }

        private void HandleSkipInput(float deltaTime)
        {
            if (Input.GetKeyDown(SkipKey))
            {
                _isHolding = true;
                _currentHoldTime = 0f;
                _view.StartHoldAnims(SkipHoldDuration);
            }

            if (Input.GetKey(SkipKey) && _isHolding)
            {
                _currentHoldTime += deltaTime;
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

            _view.SetText(replica.RawText);
            _view.SetPortrait(characterData.Portrait);
            _view.SetBackground(characterData.Background);
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