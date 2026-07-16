namespace Assets._Project.Develop.Runtime.Utilities.RandomManagment
{
    /// <summary>
    /// Детерминированная реализация IGameplayRandom на xorshift32.
    ///
    /// Почему СВОЙ PRNG, а не System.Random: алгоритм System.Random не
    /// зафиксирован спецификацией и уже менялся между .NET Framework и .NET Core.
    /// Для реплеев это скрытая мина — запись, снятая на одном рантайме, могла бы
    /// не воспроизвестись на другом при том же seed. xorshift32 задан здесь явно,
    /// поэтому последовательность стабильна между билдами и платформами.
    ///
    /// Внутри НЕТ ни одного обращения к UnityEngine.Random — иначе детерминизма
    /// от seed не было бы вовсе.
    /// </summary>
    public class GameplayRandom : IGameplayRandom
    {
        private uint _state;

        public GameplayRandom(int seed)
        {
            _state = unchecked((uint)seed);

            // xorshift32 вырождается в ноль навсегда, если состояние стало нулём.
            // seed == 0 — легальный вход снаружи, поэтому подменяем на константу.
            if (_state == 0)
            {
                _state = 0x9E3779B9;
            }
        }

        /// <summary>
        /// [0, 1], обе границы включены. NextUInt() никогда не выдаёт 0 (состояние
        /// xorshift32 всегда ненулевое), поэтому диапазон источника — [1, uint.MaxValue];
        /// сдвигаем на единицу, чтобы ноль был достижим и границы совпали с
        /// UnityEngine.Random.value.
        /// </summary>
        public float Value => (NextUInt() - 1u) / (float)(uint.MaxValue - 1u);

        /// <summary>
        /// [min, max], максимум ВКЛЮЧЁН — семантика UnityEngine.Random.Range(float, float).
        /// Достигается тем, что Value включает единицу.
        /// </summary>
        public float Range(float min, float max)
        {
            return min + (max - min) * Value;
        }

        /// <summary>
        /// [min, max), максимум ИСКЛЮЧЁН — семантика UnityEngine.Random.Range(int, int).
        /// Пустой или вывернутый диапазон возвращает min, как и Unity.
        /// </summary>
        public int Range(int min, int max)
        {
            if (max <= min)
            {
                return min;
            }

            // long, чтобы span не переполнился на широких диапазонах.
            long span = (long)max - min;

            // Остаток даёт пренебрежимо малый modulo-bias при span << 2^32,
            // а реальные диапазоны здесь — веса лут-таблицы и счётчики.
            return (int)(min + NextUInt() % (uint)span);
        }

        private uint NextUInt()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;

            return _state;
        }
    }
}
