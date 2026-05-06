using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.TextFeatures
{
    public static class TextHighlightUtility
    {
        private static readonly Dictionary<string, string> Keywords = new()
        {
            { "CAREFULLY", "#FF0000" },
            { "PIZZA", "#FFD700" },
            { "NINJA", "#00FF00" },
            { "NINJA GIRL", "#FF9EC4" },
            { "TIPS", "#00FF00" },
            { "FAST", "#FF6B00" },
            { "SECRETS", "#FF0031" },
            { "DANGEROUS", "#FF0013" },
        };

        public static string ProcessText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string cleanWord = words[i].Trim(',', '.', '!', '?', ':').ToUpper();

                if (Keywords.TryGetValue(cleanWord, out string color))
                {
                    words[i] = words[i].Replace(words[i].Trim(',', '.', '!', '?', ':'),
                        $"<color={color}>{cleanWord}</color>");
                }
            }

            return string.Join(" ", words);
        }
    }
}