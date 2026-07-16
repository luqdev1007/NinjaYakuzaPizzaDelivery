namespace Assets._Project.Develop.Runtime.Utilities.RandomManagment
{
    /// <summary>
    /// Создаёт засеянные экземпляры IGameplayRandom. Паттерн — как у
    /// TimerServiceFactory: сама фабрика живёт в DI синглтоном, а инстансы
    /// создаются мимо DI-lifetime (CLAUDE.md: массовое создание — через *Factory).
    ///
    /// Контейнер фабрике не нужен — GameplayRandom не имеет зависимостей, поэтому
    /// конструктор пустой (в отличие от TimerServiceFactory, которому нужен
    /// ICoroutinesPerformer).
    /// </summary>
    public class RandomFactory
    {
        public IGameplayRandom Create(int seed)
        {
            return new GameplayRandom(seed);
        }
    }
}
