using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set the type of interactors to interact with by changing the "interactors" field
[Interaction(interactable: typeof(Teleportor), interactors: typeof(GeneralInteractor))]
public class Teleportor : InteractableObject
{
    [SerializeField] private Transform destination;
    [SerializeField] private RefString promptText;
    private InteractionPrompt prompt;

    private new void Start()
    {
        base.Start();
        prompt = new InteractionPrompt();
        prompt.Primary = promptText.Value;
    }

    // Teleport the interactor to the destination
    public override void OnInteract(Interactor interactor, InteractionType interactType)
    {
        Transform interactorTransform = interactor.GetComponent<Transform>();
        interactorTransform.position = destination.position;
    }

    public override InteractionPrompt GetInteractionOptions(Type t)
    {
         return prompt;
    }
}
