using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set the type of interactors to interact with by changing the "interactors" field
[Interaction(interactable:typeof(Teleportor), interactors:typeof(GeneralInteractor))] 
public class Teleportor : InteractableObject
{
    // TODO: Add the required fields here

    // Teleport the interactor to the destination
    public override string OnInteract(Interactor interactor, InteractionType interactType)
    {
        // TODO: Implement this
        return Setting.INTERACTION_OK;
    }

    public override Dictionary<InteractionType, string> GetInteractionOptions(Type t)
    {
        throw new NotImplementedException();
    }
}
