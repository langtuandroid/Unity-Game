using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueResponse
{
    [TextArea]
    [SerializeField] private string text;
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private Operation operation;

    public void Respond() {
        if (operation != null) {
            operation.Start();
        }
    }

    public DialogueObject Dialogue {
        get { return dialogue; }
    }

    public string Text {
        get { return text; }
    }
}
