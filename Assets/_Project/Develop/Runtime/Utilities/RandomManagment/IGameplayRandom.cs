namespace Assets._Project.Develop.Runtime.Utilities.RandomManagment
{
    /// <summary>
    /// Детерминированный источник случайности для ГЕЙМПЛЕЙНОЙ ветки: направление
    /// движения врагов, дроп лута, множители валюты, проки в бою. Одинаковый seed
    /// даёт одинаковую последовательность — это фундамент под запись реплеев.
    ///
    /// Визуальный рандом (глитч, анимации, партиклы, UI) СОЗНАТЕЛЬНО сюда не идёт
    /// и остаётся на глобальном UnityEngine.Random: детерминизм ему не нужен, а
    /// разводка потоков достигается именно тем, что геймплей ушёл отсюда.
    ///
    /// Контракт минимальный — ровно то, что реально используется в геймплейной
    /// ветке. Ни insideUnitCircle, ни rotation, ни ColorHSV в проекте не
    /// употребляются нигде, поэтому их здесь нет.
    /// </summary>
    public interface IGameplayRandom
    {
        /// <summary>Случайное значение в [0, 1] — обе границы ВКЛЮЧЕНЫ, как UnityEngine.Random.value.</summary>
        float Value { get; }

        /// <summary>
        /// Случайное float-значение в [min, max]. Максимум ВКЛЮЧЁН — как у
        /// UnityEngine.Random.Range для float.
        /// </summary>
        float Range(float min, float max);

        /// <summary>
        /// Случайное int-значение в [min, max). Максимум ИСКЛЮЧЁН — как у
        /// UnityEngine.Random.Range для int. На это завязан взвешенный выбор в
        /// DropLootService: Range(0, totalWeight) обязан не возвращать totalWeight,
        /// иначе выбор вылетит за границу таблицы.
        /// </summary>
        int Range(int min, int max);
    }
}
