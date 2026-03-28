using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets._Project.Develop.Runtime.Configs.Dialog
{
    [CreateAssetMenu(fileName = "CharactersConfig", menuName = "Configs/Dialogs/CharactersConfig")]
    public class CharactersConfig : ScriptableObject
    {
        [SerializeField] private List<CharacterData> _characters;

        public CharacterData GetCharacter(string id)
        {
            var character = _characters.FirstOrDefault(c => c.Id == id);

            if (character == null)
            {
                Debug.LogError($"[CharactersConfig] Character with ID '{id}' not found! Проверь конфиг в Assets.");
                return _characters.FirstOrDefault(); // Возвращаем первого по дефолту, чтоб не упасть
            }

            return character;
        }
    }

    [Serializable]
    public class CharacterData
    {
        [Tooltip("ID должен совпадать с тем, что указан в Replicas в LevelConfig")]
        public string Id;
        public string Name;
        public Sprite Portrait;
        public Sprite Background;
    }

}