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

    public void SetButton(Button button, TMP_Text textContainer) {
        // button.onClick.AddListener(response.Start);
        textContainer.text = text;
    }

    public virtual void Respond() { 
    }
}
