using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
    public class DialogueObject : ScriptableObject
    {
        [SerializeField] private DialogueNode[] dialogueNodes;
        [SerializeField] private DialogueResponse[] responses;
        [SerializeField] private VoidEventChannel finisherChannel;

        public DialogueNode[] Nodes
        {
            get { return (DialogueNode[])dialogueNodes.Clone(); }
        }

        public DialogueResponse[] Responses
        {
            get { return (DialogueResponse[])responses.Clone(); }
        }

        public bool HasResponses
        {
            get { return responses != null && responses.Length > 0; }
        }

        public VoidEventChannel FinisherChannel
        {
            get { return finisherChannel; }
        }
    }
}
