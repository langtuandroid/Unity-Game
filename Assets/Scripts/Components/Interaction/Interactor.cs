using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class Interactor : MonoBehaviour

{
    /* General Interactor class for InteractableObject to recognize. Inherit from this class to support custom interaction behaviors.
     */
    private Dictionary<Type, ManagedSet<InteractableObject>> interactions = new();
    private ManagedSet<InteractableObject> interactObjs = new();

    public void Awake()
    {
        InteractionAttribute.AddInteractor(this);
    }

    public void OnEnable()
    {
        InteractionAttribute.AddInteractor(this);
    }

    public void OnDisable()
    {
        InteractionAttribute.RemoveInteractor(this);
    }

    protected virtual void OnInteract(InteractableObject obj) { }

    public bool AddInteractable(InteractableObject obj) {
        Type t = obj.GetType();
        if (!interactions.ContainsKey(t)) {
            interactions[t] = new();
        }
        if (interactions[t].Add(obj)) {
            return interactObjs.Add(obj);
        }
        return false;  
    }

    public bool RemoveInteractable(InteractableObject obj) {
        Type t = obj.GetType();
        if (!interactions.ContainsKey(t))
        {
            return false;
        }
        if (interactions[t].Remove(obj)) {
            return interactObjs.Remove(obj);
        }
        return false;
    }

    public bool Interact(Type interactableType, InteractionType interactType) {
        if (!interactions.ContainsKey(interactableType)) {
            return false;
        }
        InteractableObject obj = interactions[interactableType].CurrentItem;
        if (obj == null)
        {
            return false;
        }
        OnInteract(obj);
        obj.Interact(this, interactType);
        return false;
    }

    public bool Interact(InteractionType interactType) {
        InteractableObject obj = interactObjs.CurrentItem;
        if (obj == null) {
            return false;
        }
        OnInteract(obj);
        obj.Interact(this, interactType);
        return false;
    }

    public Dictionary<InteractionType, string> GetInteractionOptions() {
        InteractableObject obj = interactObjs.CurrentItem;
        if (obj != null) {
            return obj.GetInteractionType(GetType());
        }
        return null;
    }

    public void SwitchInteractable() { 
        interactObjs.Advance();
    }
}
