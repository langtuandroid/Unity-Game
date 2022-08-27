using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set the type of interactors to interact with by changing the "interactors" field
[Interaction(interactable: typeof(Teleportor), interactors: typeof(GeneralInteractor))]
public class Teleportor : InteractableObject
{
    [SerializeField] private Transform Destination;
    private readonly Dictionary<InteractionType, string> option = new() {
        { InteractionType.Primary, "Teleport"}
    };
    // Teleport the interactor to the destination
    public override string OnInteract(Interactor interactor, InteractionType interactType)
    {
        Transform interactorTransform = interactor.GetComponent<Transform>();
        interactorTransform.position = Destination.position;
        return Setting.INTERACTION_OK;
    }

    public override Dictionary<InteractionType, string> GetInteractionOptions(Type t)
    {
         return option;
    }
}
