using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Extensions
{
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