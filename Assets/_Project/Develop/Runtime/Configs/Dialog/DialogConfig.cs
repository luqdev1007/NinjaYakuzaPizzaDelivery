using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Dialog
{
    [CreateAssetMenu]
    public class DialogConfig : ScriptableObject
    {
        public List<DialogReplica> Replicas;
    }

    [Serializable]
    public class DialogReplica
    {
        public string CharacterId;
        [TextArea] public string RawText;
        public bool OverrideTime;
        public float CustomTime;
    }
}
