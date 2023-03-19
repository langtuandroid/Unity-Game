using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LobsterFramework.DialogueSystem
{
    [System.Serializable]
    public class DialogueResponse
    {
        [TextArea]
        [SerializeField] private string text;
        [SerializeField] private DialogueObject dialogue;
        [SerializeField] private VoidEventChannel operationChannel;

        public void Respond()
        {
            if (operationChannel != null)
            {
                operationChannel.RaiseEvent();
            }
        }

        public DialogueObject Dialogue
        {
            get { return dialogue; }
        }

        public string Text
        {
            get { return text; }
        }
    }
}
