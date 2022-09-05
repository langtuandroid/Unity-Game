using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [Header("Interactor to display prompt for")]
    public Interactor interactor;

    [Header("Interaction Prompts")]
    [SerializeField] private TMP_Text interactionPrompt1;
    [SerializeField] private TMP_Text interactionPrompt2;
    [SerializeField] private TMP_Text interactionPrompt3;
    [SerializeField] private TMP_Text interactionPrompt4;

    [Header("Interaction Response")]
    [SerializeField] private StringEventChannel interactionChannel;
    [SerializeField] private float displaySpeed;
    [SerializeField] private float timeLingering;
    [SerializeField] private GameObject responseArea;
    [SerializeField] private TMP_Text responseText;

    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = null;
    }

    private void OnEnable()
    {
        interactionChannel.OnEventRaised += DisplayInteractionResponse;
    }

    private void OnDisable()
    {
        interactionChannel.OnEventRaised += DisplayInteractionResponse;
    }

    private void LateUpdate()
    {
        if (interactionPrompt1 == null)
        {
            return;
        }
        DisplayInteractionPrompt();
    }

    private void DisplayInteractionPrompt()
    {
        InteractionPrompt prompt = interactor.GetInteractionOptions();

        if (prompt.Primary != default)
        {
            interactionPrompt1.text = prompt.Primary;
            interactionPrompt1.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt1.gameObject.SetActive(false);
        }

        if (prompt.Secondary != default)
        {
            interactionPrompt2.text = prompt.Secondary;
            interactionPrompt2.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt2.gameObject.SetActive(false);
        }

        if (prompt.Tertiary != default)
        {
            interactionPrompt3.text = prompt.Tertiary;
            interactionPrompt3.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt3.gameObject.SetActive(false);
        }

        if (prompt.Quaternary != default)
        {
            interactionPrompt4.text = prompt.Quaternary;
            interactionPrompt4.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt4.gameObject.SetActive(false);
        }
    }

    private void DisplayInteractionResponse(string response) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DisplayMessage(response));
    }

    private IEnumerator DisplayMessage(string message) {
        responseArea.SetActive(true);
        float index = 0;
        while (index < message.Length)
        {
            responseText.text = message.Substring(0, Mathf.FloorToInt(index));
            index += Time.deltaTime * displaySpeed;
            yield return null;
        }
        responseText.text = message;
        yield return new WaitForSeconds(timeLingering);
        responseArea.SetActive(false);
    }
}
