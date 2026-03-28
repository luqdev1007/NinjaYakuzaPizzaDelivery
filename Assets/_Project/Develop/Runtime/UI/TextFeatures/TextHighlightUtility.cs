using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Develop.Runtime.UI.TextFeatures
{
    public static class TextHighlightUtility
    {
        // Можно заменить на загрузку из твоего конфига
        private static readonly Dictionary<string, string> Keywords = new()
        {
            { "CAREFULLY", "#FF0000" }, // Red
            { "PIZZA", "#FFD700" },     // Gold
            { "NINJA", "#00FF00" }      // Green
        };

        public static string ProcessText(string input)
        {
            foreach (var pair in Keywords)
            {
                if (input.Contains(pair.Key))
                    input = input.Replace(pair.Key, $"<color={pair.Value}>{pair.Key}</color>");
            }
            return input;
        }
    }
}
