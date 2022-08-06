using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactor : MonoBehaviour
{
    private Dictionary<Type, InteractionManager> interactions = new();

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

    public bool AddInteractable(InteractableObject obj) {
        Type t = obj.GetType();
        if (!interactions.ContainsKey(t)) {
            interactions[t] = new();
        }
        return interactions[t].AddInteractable(obj);
    }

    public bool RemoveInteractable(InteractableObject obj) {
        Type t = obj.GetType();
        if (!interactions.ContainsKey(t))
        {
            return false;
        }
        return interactions[t].RemoveInteractable(obj);
    }

    public bool Interact(Type interactableType) {
        if (!interactions.ContainsKey(interactableType)) {
            return false;
        }
        return interactions[interactableType].Interact(this);
    }

    private class InteractionManager
    {
        private List<InteractableObject> interactables = new();
        private int pointer = 0;

        private int Pointer
        {
            get
            {
                if (interactables.Count == 0)
                {
                    return -1;
                }
                if (pointer >= interactables.Count || pointer < 0)
                {
                    pointer = interactables.Count - 1;
                }

                return pointer;
            }
        }
        public InteractableObject Interacting
        {
            get
            {
                if (Pointer == -1)
                {
                    return null;
                }
                return interactables[pointer];
            }
        }

        public bool Interact(Interactor interactor)
        {
            if (Pointer == -1)
            {
                return false;
            }
            interactables[pointer].Interact(interactor);
            return true;
        }

        public void SwitchInteractble()
        {
            pointer++;
        }

        public bool AddInteractable(InteractableObject i)
        {
            Debug.Log("Adding " + i.gameObject.name);
            if (!interactables.Contains(i))
            {
                interactables.Add(i);
                return true;
            }
            return false;
        }

        public bool RemoveInteractable(InteractableObject i)
        {
            Debug.Log("Removing " + i.gameObject.name);
            if (interactables.Remove(i))
            {
                return true;
            }

            return false;
        }
    }
}
