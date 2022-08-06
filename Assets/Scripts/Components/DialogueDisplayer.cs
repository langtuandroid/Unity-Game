using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Interaction(interactable :typeof(DialogueDisplayer), interactors: typeof(PlayerController))] 
public class DialogueDisplayer : InteractableObject
{
    [SerializeField] private RefBool resetUponFinish;
    [SerializeField] private TMP_Text container;
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private RefFloat displaySpeed;
    [SerializeField] private RefFloat secondsDelayBetweenLines;
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private Button responseButton;

    private Coroutine coroutine;
    private DialogueObject currentDialogue;

    private List<Button> responseButtons = new();

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
            currentDialogue = dialogue;
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

        if (currentDialogue.HasResponses) {
            DialogueResponse[] responses = currentDialogue.Responses;
            foreach (DialogueResponse response in responses)
            {
                GameObject obj = Instantiate(responseButton.gameObject, responseBox);
                obj.SetActive(true);
                Button btn = obj.GetComponent<Button>();
                btn.onClick.AddListener(() => Respond(response));
                btn.GetComponentInChildren<TMP_Text>().text = response.Text; 
                responseButtons.Add(btn);
            }
        }
        if (resetUponFinish) {
            currentDialogue = dialogue;
        }
        coroutine = null;
    }

    public void Respond(DialogueResponse response) { 
        response.Respond();
        if (response.Dialogue != null)
        {
            currentDialogue = response.Dialogue;
            coroutine = StartCoroutine(DisplayText());
        }
        else {
            currentDialogue = dialogue;
        }
        
        foreach (Button b in responseButtons) {
            Destroy(b.gameObject);
        }
        responseButtons.Clear();
    }
}
