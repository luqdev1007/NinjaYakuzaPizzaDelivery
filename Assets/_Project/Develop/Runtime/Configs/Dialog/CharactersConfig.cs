using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Configs.Dialog
{
    public enum CharacterIDs
    {
        None = 0,
        MainHero = 1,
        Boss = 2,
        RandomClient = 3,
        NinjaGirl = 4,
    }

    [CreateAssetMenu(fileName = "CharactersConfig", menuName = "Configs/Dialogs/CharactersConfig")]
    public class CharactersConfig : ScriptableObject
    {
        [SerializeField] private List<CharacterData> _characters;

        public CharacterData GetCharacter(CharacterIDs id)
        {
            CharacterData character = _characters.Find(c => c.Id == id);

            if (character == null)
                throw new NullReferenceException($"[CharactersConfig] Character with ID '{id}' not found!");

            return character;
        }
    }

    [Serializable]
    public class CharacterData
    {
        public CharacterIDs Id;
        public string Name;
        public Sprite Portrait;
        public Sprite Background;
    }
}