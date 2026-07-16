using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagement
{
    public class GameplayInputArgs : IInputSceneArgs
    {
        public GameplayInputArgs(int levelNumber)
        {
            LevelNumber = levelNumber;
        }

        public int LevelNumber { get; private set; }
        public bool IsRestart;

        /// <summary>
        /// Seed геймплейного потока рандома на текущий забег. Живёт здесь, потому что
        /// GameplayInputArgs — единственный объект, который переживает рестарт уровня
        /// (инстанс создаётся один раз на тайл в LevelsMenuPopupPresenter) и доезжает
        /// до бутстрапа через sceneArgs; прецедент изменяемого per-run поля рядом —
        /// IsRestart.
        ///
        /// Пишется в GameplayBootstrap.Initialize() ПЕРЕД регистрациями, читается в
        /// GameplayContextRegistrations при создании IGameplayRandom. Новый seed на
        /// каждый вход в уровень, включая рестарт: рестарт — это новый забег
        /// (сбрасываются таймер и лут сессии), а не воспроизведение старого.
        ///
        /// Под запись реплея: именно это значение должно попасть в запись, чтобы
        /// забег воспроизвёлся.
        /// </summary>
        public int Seed;
    }
}
