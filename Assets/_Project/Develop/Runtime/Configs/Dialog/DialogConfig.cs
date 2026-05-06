using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Dialog
{
    [CreateAssetMenu(fileName = "CharactersConfig", menuName = "Configs/Dialogs/DialogConfig")]
    public class DialogConfig : ScriptableObject
    {
        public List<DialogReplica> Replicas;
    }

    [Serializable]
    public class DialogReplica
    {
        [TextArea] public string RawText;
        public CharacterIDs CharacterId;
    }
}
