using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;

namespace Assets._Project.Develop.Runtime.Utilites.SceneManagement
{
    public class GameplayInputArgs : IInputSceneArgs
    {
        public GameplayInputArgs(LevelConfig levelConfig)
        {
            LevelConfig = levelConfig;
        }

        public LevelConfig LevelConfig { get; private set; }
    }
}
