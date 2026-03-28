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
        private bool _isTyping;
        private bool _skipRequested;

        public DialogPresenter(
            DialogDisplayView view,
            DialogConfig config,
            ICoroutinesPerformer coroutines,
            CharactersConfig charactersConfig)
        {
            _view = view;
            _config = config;
            _coroutines = coroutines;
            _charactersConfig = charactersConfig;
        }

        public void Initialize()
        {
            _view.Show();
            // Запускаем одну главную корутину всего диалога
            _dialogSequence = _coroutines.StartPerform(DialogRoutine());
        }

        public void Update(float deltaTime)
        {
            // Слушаем нажатие E для скипа
            if (Input.GetKeyDown(KeyCode.E))
            {
                _skipRequested = true;
            }
        }

        private IEnumerator DialogRoutine()
        {
            for (int i = 0; i < _config.Replicas.Count; i++)
            {
                var replica = _config.Replicas[i];
                _skipRequested = false;

                // 1. Настройка визуала
                var character = _charactersConfig.GetCharacter(replica.CharacterId);
                _view.SetPortrait(character.Portrait);
                _view.SetBackground(character.Background);

                // 2. Печать текста
                string processedText = TextHighlightUtility.ProcessText(replica.RawText);
                yield return _coroutines.StartPerform(TypewriterEffect(processedText));

                // 3. Ожидание нажатия E для перехода к следующей реплике
                Debug.Log($"[Dialog] Replica {i} finished typing. Waiting for 'E'...");

                // Ждем, пока игрок нажмет E (флаг поднимется в Update)
                yield return new WaitUntil(() => _skipRequested);

                // Сбрасываем флаг для следующей итерации
                _skipRequested = false;

                // Небольшая задержка, чтобы одно нажатие не проскочило две реплики
                yield return new WaitForSeconds(0.1f);
            }

            FinishDialog();
        }

        private IEnumerator TypewriterEffect(string fullText)
        {
            _isTyping = true;
            _view.SetText(fullText);

            var text = _view.СontentProgressText;

            // --- ВАЖНО: Принудительное обновление меша ---
            text.maxVisibleCharacters = 0;
            text.ForceMeshUpdate(); // Заставляем TMP рассчитать текст прямо сейчас

            // Даем один кадр на всякий случай
            yield return null;

            int totalChars = text.textInfo.characterCount;

            // Если после ForceMeshUpdate все еще 0, берем длину строки напрямую
            if (totalChars == 0 && fullText.Length > 0)
                totalChars = fullText.Length;

            Debug.Log($"[Typewriter] Typing started. Total chars: {totalChars}");

            int counter = 0;
            while (counter <= totalChars)
            {
                if (_skipRequested)
                {
                    text.maxVisibleCharacters = totalChars;
                    // Не сбрасываем _skipRequested здесь, чтобы основная корутина увидела нажатие
                    break;
                }

                text.maxVisibleCharacters = counter;
                counter++;
                yield return new WaitForSeconds(0.03f);
            }

            _isTyping = false;
        }

        private void FinishDialog()
        {
            _view.Hide();
            DialogEnded?.Invoke();
        }

        public void Dispose()
        {
            if (_dialogSequence != null)
                _coroutines.StopPerform(_dialogSequence);

            DialogEnded = null;
        }
    }
}