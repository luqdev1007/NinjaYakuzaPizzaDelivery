using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.TextFeatures;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogPresenter : IPresenter
    {
        private readonly DialogDisplayView _view;
        private readonly DialogConfig _config;
        private readonly TimerService _autoNextTimer;
        private readonly ICoroutinesPerformer _coroutines;

        private int _currentReplicaIndex = -1;
        private bool _isTyping;
        private string _fullProcessedText;
        private Coroutine _typingCoroutine;

        private float _eHoldTime = 0f;
        private bool _isAppearanceDone = false;

        public DialogPresenter(DialogDisplayView view, DialogConfig config, TimerServiceFactory timerFactory, ICoroutinesPerformer coroutines)
        {
            _view = view;
            _config = config;
            _coroutines = coroutines;
            _autoNextTimer = timerFactory.Create(1f); // Время выставим позже
        }

        public void Initialize()
        {
            _view.AppearanceFinished += OnAppearanceFinished;
            _autoNextTimer.CooldownEnded.Subscribe(NextReplica);
            _view.Show();
        }

        private void OnAppearanceFinished()
        {
            _isAppearanceDone = true;
            NextReplica();
        }

        public void Update(float deltaTime)
        {
            if (!_isAppearanceDone) 
                return;

            HandleInput(deltaTime);
        }

        private void HandleInput(float deltaTime)
        {
            if (Input.GetKey(KeyCode.E))
            {
                _eHoldTime += deltaTime;
                if (_eHoldTime >= 3f) SkipFullDialog();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isTyping)
                {
                    FinishTypingImmediately();
                }
                else
                {
                    NextReplica();
                }
            }

            if (Input.GetKeyUp(KeyCode.E)) _eHoldTime = 0f;
        }

        private void NextReplica()
        {
            _currentReplicaIndex++;
            if (_currentReplicaIndex >= _config.Replicas.Count)
            {
                FinishDialog();
                return;
            }

            var replica = _config.Replicas[_currentReplicaIndex];

            // Подготовка текста и визуалов
            _fullProcessedText = TextHighlightUtility.ProcessText(replica.RawText);
            // _view.SetPortrait(...) - тут достаешь из своего CharacterConfig

            // Запуск печатной машинки
            if (_typingCoroutine != null) _coroutines.StopPerform(_typingCoroutine);
            _typingCoroutine = _coroutines.StartPerform(TypewriterEffect(_fullProcessedText));

            // Настройка таймера авто-перехода
            float readTime = replica.OverrideTime ? replica.CustomTime : _fullProcessedText.Length * 0.1f;
            // Перезапуск таймера с новым временем (нужен метод SetCooldown в TimerService)
        }

        private IEnumerator TypewriterEffect(string text)
        {
            _isTyping = true;
            _view.SetText("");

            // Умная печать (игнорируем теги)
            int visibleChars = 0;
            while (visibleChars <= text.Length)
            {
                // Здесь можно использовать maxVisibleCharacters у TMP для простоты
                _view.SetText(text);
                // Но лучше посимвольно, пропуская < > теги
                visibleChars++;
                yield return new WaitForSeconds(0.03f);
            }

            _isTyping = false;
            _autoNextTimer.Restart();
        }

        private void FinishTypingImmediately()
        {
            _coroutines.StopPerform(_typingCoroutine);
            _view.SetText(_fullProcessedText);
            _isTyping = false;
            _autoNextTimer.Restart();
        }

        private void SkipFullDialog() => FinishDialog();

        private void FinishDialog()
        {
            _autoNextTimer.Stop();
            _view.Hide();
            // Event: DialogEnded?.Invoke();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}

