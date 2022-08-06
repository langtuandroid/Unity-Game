using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [TextArea]
    [SerializeField] private string[] texts;
    [SerializeField] private DialogueResponse[] responses;

    public string[] Texts { 
        get { return (string[])texts.Clone(); }
    }

    public DialogueResponse[] Responses {
        get { return (DialogueResponse[])responses.Clone(); }
    }

    public bool HasResponses
    {
        get { return responses != null && responses.Length > 0; }
    }
}
