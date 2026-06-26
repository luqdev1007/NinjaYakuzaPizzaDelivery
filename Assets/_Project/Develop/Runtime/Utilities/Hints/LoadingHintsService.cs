using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Hints
{
    public class LoadingHintsService : ILoadingHintsService
    {
        private const string HintsResourcePath = "Content/loading_hints";

        private List<LoadingHintEntry> _hints;

        public string GetRandomHint()
        {
            EnsureLoaded();

            if (_hints.Count == 0)
            {
                return string.Empty;
            }

            var index = Random.Range(0, _hints.Count);
            return _hints[index].text;
        }

        private void EnsureLoaded()
        {
            if (_hints != null)
            {
                return;
            }

            var textAsset = Resources.Load<TextAsset>(HintsResourcePath);
            if (textAsset == null)
            {
                Debug.LogError($"[LoadingHintsService] Hints file not found: Resources/{HintsResourcePath}.json");
                _hints = new List<LoadingHintEntry>();
                return;
            }

            var wrapper = JsonUtility.FromJson<LoadingHintsWrapper>(textAsset.text);
            _hints = wrapper?.hints ?? new List<LoadingHintEntry>();
        }
    }
}
