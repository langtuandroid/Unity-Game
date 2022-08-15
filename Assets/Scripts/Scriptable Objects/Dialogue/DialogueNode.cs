using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    [SerializeField] private Sprite speakerIcon;
    [SerializeField] private string speaker;
    [TextArea]
    [SerializeField] private string[] texts;

    public string[] Texts
    {
        get { return (string[])texts.Clone(); }
    }

    public Sprite Icon
    {
        get { return speakerIcon; }
    }

    public string Speaker { 
        get { return speaker; }
    }
}
