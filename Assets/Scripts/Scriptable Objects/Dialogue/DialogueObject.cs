using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DialogueNode[] dialogueNodes;
    [SerializeField] private DialogueResponse[] responses;
    [SerializeField] private OperationReference[] finishers;

    public DialogueNode[] Nodes { 
        get { return (DialogueNode[])dialogueNodes.Clone(); }
    }

    public DialogueResponse[] Responses {
        get { return (DialogueResponse[])responses.Clone(); }
    }

    public bool HasResponses
    {
        get { return responses != null && responses.Length > 0; }
    }

    public bool HasFinishers { 
        get { return finishers != null && finishers.Length > 0; }
    }

    public void ExecuteFinisher() {
        if (finishers != null) {
            foreach (OperationReference op in finishers) {
                op.Operate();
            }
        }
    }
}
