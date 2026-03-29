using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.TextFeatures;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogPresenter : IPresenter, IDisposable
    {
        public event Action DialogEnded;

        private readonly DialogDisplayView _view;
        private readonly DialogConfig _config;
        private readonly CharactersConfig _charactersConfig;
        private readonly ICoroutinesPerformer _coroutines;

        private Coroutine _dialogSequence;
        private bool _isAppeared;

        private float _holdTime;
        private const float MaxHoldTime = 1.2f; // Немного ускорил для динамики
        private bool _fullSkipTriggered;
        private bool _skipRequested;

        public DialogPresenter(DialogDisplayView view, DialogConfig config,
            ICoroutinesPerformer coroutines, CharactersConfig charactersConfig)
        {
            _view = view;
            _config = config;
            _coroutines = coroutines;
            _charactersConfig = charactersConfig;
        }

        public void Initialize()
        {
            _view.AppearanceFinished += OnAppearanceFinished;
            _view.Show();
        }

        private void OnAppearanceFinished()
        {
            _view.AppearanceFinished -= OnAppearanceFinished;
            _isAppeared = true;
            _dialogSequence = _coroutines.StartPerform(DialogRoutine());
        }

        public void Update(float deltaTime)
        {
            if (!_isAppeared || _fullSkipTriggered) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                _skipRequested = true;
                _view.StartHoldAnims(MaxHoldTime);
            }

            if (Input.GetKey(KeyCode.E))
            {
                _holdTime += deltaTime;
                if (_holdTime >= MaxHoldTime)
                {
                    _fullSkipTriggered = true;
                    _view.ExplodeSkip();
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                _holdTime = 0;
                _view.StopHoldAnims();
            }
        }

        private IEnumerator DialogRoutine()
        {
            for (int i = 0; i < _config.Replicas.Count; i++)
            {
                if (_fullSkipTriggered) break;

                var replica = _config.Replicas[i];
                _skipRequested = false;

                _view.SetPortrait(_charactersConfig.GetCharacter(replica.CharacterId).Portrait);
                _view.SetBackground(_charactersConfig.GetCharacter(replica.CharacterId).Background);

                string processedText = TextHighlightUtility.ProcessText(replica.RawText);
                yield return _coroutines.StartPerform(TypewriterEffect(processedText));

                if (_fullSkipTriggered) break;

                _view.ShowSkipHint();
                yield return new WaitUntil(() => _skipRequested || _fullSkipTriggered);

                _skipRequested = false;
                yield return new WaitForSeconds(0.1f);
            }

            FinishDialog();
        }

        private IEnumerator TypewriterEffect(string fullText)
        {
            _view.SetText(fullText);
            var text = _view.СontentProgressText;
            text.maxVisibleCharacters = 0;
            text.ForceMeshUpdate();

            int totalChars = text.textInfo.characterCount > 0 ? text.textInfo.characterCount : fullText.Length;
            int counter = 0;

            while (counter <= totalChars)
            {
                if (_skipRequested || _fullSkipTriggered)
                {
                    text.maxVisibleCharacters = totalChars;
                    break;
                }

                text.maxVisibleCharacters = counter;
                counter++;
                yield return new WaitForSeconds(0.02f); // Чуть быстрее печать
            }
        }

        private void FinishDialog()
        {
            _view.Hide();
            DialogEnded?.Invoke();
        }

        public void Dispose()
        {
            _view.AppearanceFinished -= OnAppearanceFinished;
            if (_dialogSequence != null) _coroutines.StopPerform(_dialogSequence);
            DialogEnded = null;
        }
    }
}