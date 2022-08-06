using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Interaction(interactable :typeof(DialogueDisplayer), interactors: typeof(PlayerController))] 
public class DialogueDisplayer : InteractableObject
{
    [SerializeField] private TMP_Text container;
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private RefFloat displaySpeed;
    [SerializeField] private RefFloat secondsDelayBetweenLines;
    private Coroutine coroutine;

    private DialogueObject currentDialogue;

    new void Start()
    {
        base.Start();
        currentDialogue = dialogue;
    }

    public override void Interact(Interactor interactor) {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(DisplayText());
        }
        else {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public IEnumerator DisplayText() {
        foreach (string str in currentDialogue.Texts) {
            float index = 0;
            while (index < str.Length) { 
                container.text = str.Substring(0, Mathf.FloorToInt(index));
                index += Time.deltaTime * displaySpeed.Value;
                yield return null;
            }
            container.text = str;
            yield return new WaitForSeconds(secondsDelayBetweenLines.Value);
        }
        coroutine = null;
    }
}
