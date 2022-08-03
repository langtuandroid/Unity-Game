using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InteractorListSO interactors;

    private List<InteractableObject> interactables = new();
    private int pointer = 0;

    private int Pointer { 
        get {
            if (interactables.Count == 0) {
                return -1;
            }
            if (pointer >= interactables.Count || pointer < 0) {
                pointer = interactables.Count - 1;
            } 

            return pointer;
        }
    }

    public void Awake()
    {
        interactors.Add(this);
    }

    public InteractableObject Interacting {
        get {
            if (Pointer == -1) {
                return null;
            }
            return interactables[pointer]; 
        }
    }

    public void Interact() {
        if (Pointer == -1) {
            return;
        }
        interactables[pointer].Interact(this);
    }

    public void SwitchInteractble() {
        pointer++;
    }

    public bool AddInteractable(InteractableObject i) {
        Debug.Log("Adding " + i.gameObject.name);
        if (!interactables.Contains(i)) {
            interactables.Add(i);
            return true;
        }
        return false;
    }

    public bool RemoveInteractable(InteractableObject i) {
        Debug.Log("Removing " + i.gameObject.name);
        if (interactables.Remove(i)) {
            return true;
        }

        return false;
    }
}
