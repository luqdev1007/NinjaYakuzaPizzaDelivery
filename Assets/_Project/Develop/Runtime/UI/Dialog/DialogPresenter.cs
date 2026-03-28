using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.TextFeatures;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Timer;
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
        private readonly TimerServiceFactory _timerFactory;
        private readonly ICoroutinesPerformer _coroutines;

        private TimerService _autoNextTimer;
        private int _currentReplicaIndex = -1;
        private bool _isTyping;
        private Coroutine _typingCoroutine;
        private bool _isAppearanceDone;
        private float _eHoldTime = 0f;

        public DialogPresenter(
            DialogDisplayView view,
            DialogConfig config,
            TimerServiceFactory timerFactory,
            ICoroutinesPerformer coroutines,
            CharactersConfig charactersConfig)
        {
            _view = view;
            _config = config;
            _timerFactory = timerFactory;
            _coroutines = coroutines;
            _charactersConfig = charactersConfig;
        }

        public void Initialize()
        {
            _isAppearanceDone = false;
            _currentReplicaIndex = -1;
            _isTyping = false;
            _eHoldTime = 0f;

            _view.Show();
            NextReplica();
        }

        public void Update(float deltaTime)
        {
            if (!_isAppearanceDone) return;

            HandleInput(deltaTime);
        }

        private void HandleInput(float deltaTime)
        {
            // Удержание E для пропуска всего диалога
            if (Input.GetKey(KeyCode.E))
            {
                _eHoldTime += deltaTime;
                if (_eHoldTime >= 3f)
                {
                    SkipFullDialog();
                    return;
                }
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                _eHoldTime = 0f;
            }

            // Нажатие E для пропуска текста или перехода
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isTyping)
                    FinishTypingImmediately();
                else
                    NextReplica();
            }
        }

        private void NextReplica()
        {
            StopCurrentTimer();

            _currentReplicaIndex++;

            if (_currentReplicaIndex >= _config.Replicas.Count)
            {
                FinishDialog();
                return;
            }

            var replica = _config.Replicas[_currentReplicaIndex];

            // Настройка визуала персонажа
            var character = _charactersConfig.GetCharacter(replica.CharacterId);
            _view.SetPortrait(character.Portrait);
            _view.SetBackground(character.Background);

            // Подготовка текста
            string processedText = TextHighlightUtility.ProcessText(replica.RawText);

            if (_typingCoroutine != null)
                _coroutines.StopPerform(_typingCoroutine);

            _typingCoroutine = _coroutines.StartPerform(TypewriterEffect(processedText));

            // Настройка таймера авто-перехода
            float readTime = replica.OverrideTime
                ? replica.CustomTime
                : processedText.Length * 0.05f + 2f; // Базовое время на чтение

            _autoNextTimer = _timerFactory.Create(readTime);
            _autoNextTimer.CooldownEnded.Subscribe(NextReplica);
            _autoNextTimer.Restart();
        }

        private void StopCurrentTimer()
        {
            if (_autoNextTimer != null)
            {
                _autoNextTimer.Stop();
                // Если твой TimerService требует очистки подписок:
                // _autoNextTimer.CooldownEnded.Unsubscribe(NextReplica);
                _autoNextTimer = null;
            }
        }

        private IEnumerator TypewriterEffect(string fullText)
        {
            _isTyping = true;
            _view.SetText(fullText);

            var textComponent = _view.СontentProgressText;
            textComponent.maxVisibleCharacters = 0;

            // Ждем кадра, чтобы TMP расчитал количество символов
            yield return null;

            int totalVisibleCharacters = textComponent.textInfo.characterCount;
            int counter = 0;

            while (counter <= totalVisibleCharacters)
            {
                textComponent.maxVisibleCharacters = counter;
                counter++;
                yield return new WaitForSeconds(0.5f);
            }

            _isTyping = false;
        }

        private void FinishTypingImmediately()
        {
            if (_typingCoroutine != null)
            {
                _coroutines.StopPerform(_typingCoroutine);
                _typingCoroutine = null;
            }

            _view.СontentProgressText.maxVisibleCharacters = 9999;
            _isTyping = false;
        }

        private void SkipFullDialog() => FinishDialog();

        private void FinishDialog()
        {
            StopCurrentTimer();
            _view.Hide();
            DialogEnded?.Invoke();
        }

        public void Dispose()
        {
            StopCurrentTimer();


            if (_typingCoroutine != null)
                _coroutines.StopPerform(_typingCoroutine);

            DialogEnded = null;
        }
    }
}