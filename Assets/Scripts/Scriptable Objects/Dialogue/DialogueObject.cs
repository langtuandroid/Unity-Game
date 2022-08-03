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
}
