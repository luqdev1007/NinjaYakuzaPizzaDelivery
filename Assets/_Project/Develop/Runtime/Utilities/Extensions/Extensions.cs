using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Extensions
{
    /// <summary>
    /// КАНДИДАТ НА УДАЛЕНИЕ — потребителей не осталось.
    ///
    /// Единственным потребителем был DoubleAttackSystem, и он мигрировал на
    /// засеянный IGameplayRandom (собственный IsChanceProceed с той же формулой
    /// Range(0, 100) &lt;= percent). Класс сидит на ГЛОБАЛЬНОМ UnityEngine.Random,
    /// поэтому любой новый геймплейный потребитель здесь молча вернул бы утечку
    /// визуального потока в реплей-чувствительные решения — ровно то, что
    /// разделение потоков и чинило.
    ///
    /// Не удалён намеренно: grep мог не поймать вызовы через рефлексию или из кода
    /// вне Assets/_Project/Develop — решение за владельцем. Если новый вызов всё же
    /// нужен, правильный путь — IGameplayRandom, а не этот класс.
    /// </summary>
    public static class GameRandom
    {
        public static bool IsChanceProceed(float percent)
        {
            return Random.Range(0f, 100f) <= percent;
        }

        public static bool IsChanceProceed(int percent)
        {
            return Random.Range(0f, 100f) <= percent;
        }
    }
}
