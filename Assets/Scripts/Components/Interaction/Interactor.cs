using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactor : MonoBehaviour
{
    /* Interactors for InteractableObjects to recognize. Inherit from this class to support custom interaction behaviors
     * There can be multiple interactors on one game object
     * 
     * Note: The inherited classes must call base.Start() in their Start() method to ensure proper behaviors
     */
    private Dictionary<Type, ManagedSet<(InteractableObject obj, Interactor interactor)>> interactions = new();
    private ManagedSet<(InteractableObject obj, Interactor interactor)> interactObjs = new();
    private bool isBase;
    private Interactor baseInteractor;

    public bool IsBase { get { return isBase; } }

    public void Awake()
    {
        InteractionAttribute.AddInteractor(this);
    }

    public void Start()
    {
        Interactor[] interactors = GetComponents<Interactor>();
        foreach (Interactor i in interactors) {
            if (i.isBase) {
                baseInteractor = i;
                return;
            }
        }
        isBase = true;
        baseInteractor = this;
    }

    public void OnEnable()
    {
        InteractionAttribute.AddInteractor(this);
    }

    public void OnDisable()
    {
        InteractionAttribute.RemoveInteractor(this);
    }

    // Callback before interacting with the object
    protected virtual void OnInteract(InteractableObject obj, InteractionType interactionType) { }

    public bool AddInteractable(InteractableObject obj) {
        return AddInteractable(obj, this);
    }

    public bool RemoveInteractable(InteractableObject obj) {
        return RemoveInteractable(obj, this);
    }

    public bool AddInteractable(InteractableObject obj, Interactor interactor) {
        // Use this method to add interactables to the interaction list without type checking
        if (isBase) {
            Type t = obj.GetType();
            if (!interactions.ContainsKey(t))
            {
                interactions[t] = new();
            }
            if (interactions[t].Add((obj, interactor)))
            {
                return interactObjs.Add((obj, interactor));
            }
            return false;
        }
        // Redirect interatable to base interactor
        return baseInteractor.AddInteractable(obj, this);
    }

    public bool RemoveInteractable(InteractableObject obj, Interactor interactor) {
        // Use this method to remove interactables from the interaction list without type checking
        if (isBase) {
            Type t = obj.GetType();
            if (!interactions.ContainsKey(t))
            {
                return false;
            }
            if (interactions[t].Remove((obj, interactor)))
            {
                return interactObjs.Remove((obj, interactor));
            }
            return false;
        }
        // Redirect interatable to base interactor
        return baseInteractor.RemoveInteractable(obj, this);
    }

    /* The following two methods are used for managed interactions, which means the interations are subject to
     * restrictions defined by interactables themselves (i.e Interaction Range)
     * 
     * 1st Method: Interact with the current item of in the managed list of specified type
     * 2nd Method: Interact with the current item of the managed list
     * 
     * To forcefully interact with specific interactables, use the 3rd method below instead
     */
    public bool Interact(Type interactableType, InteractionType interactType) {
        if (!isBase)
        {
            return baseInteractor.Interact(interactableType, interactType);
        }
        if (!interactions.ContainsKey(interactableType)) {
            return false;
        }
        InteractableObject obj = interactions[interactableType].CurrentItem.obj;
        if (obj == null)
        {
            return false;
        }
        OnInteract(obj, interactType);
        obj.OnInteract(this, interactType);
        return false;
    }

    public bool Interact(InteractionType interactType) {
        if (!isBase) {
            return baseInteractor.Interact(interactType);
        }
        (InteractableObject, Interactor) tuple = interactObjs.CurrentItem;
        if (tuple.Item1 == null || tuple.Item2 == null) {
            return false;
        }
        if (tuple.Item2 == this) {
            OnInteract(tuple.Item1, interactType);
            tuple.Item1.OnInteract(this, interactType);
            return false;
        }
        tuple.Item2.Interact(interactType, tuple.Item1);
        return true;
    }

    // Use this method to force interaction
    public void Interact(InteractionType interactType, InteractableObject interactable) {
        OnInteract(interactable, interactType);
        interactable.OnInteract(this, interactType);
    }


    // Method for retrieving interaction info used for UI
    public InteractionPrompt GetInteractionOptions() {
        if (!isBase) {
            return baseInteractor.GetInteractionOptions();
        }
        InteractableObject obj = interactObjs.CurrentItem.obj;
        if (obj != null) {
            return obj.GetInteractionOptions(GetType());
        }
        return default;
    }

    // Move to the next interactable on the list
    public void NextInteractable() {
        if (isBase)
        {
            interactObjs.Advance();
        }
        else {
            baseInteractor.NextInteractable();
        }
    }

    // Move to the previous interactable on the list
    public void PreviousInteractable()
    {
        if (isBase)
        {
            interactObjs.StepBack();
        }
        else
        {
            baseInteractor.PreviousInteractable();
        }
    }
}
