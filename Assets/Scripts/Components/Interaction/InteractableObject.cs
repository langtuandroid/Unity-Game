using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InteractionType { 
    Primary,
    Secondary,
    Tertiary,
    Quaternary
}

public abstract class InteractableObject : MonoBehaviour
{
    /* Base Class for interactable objects, inherit from this class to implement custom interaction behaviors.
     * 
     * Note: The derived class must call base.Start() to allow the base class to complete all the necessary setups
     */

    [SerializeField] private RefFloat interactRadius;
    private Dictionary<Type, InteractionTracer> interactionTracers = new();
    private Transform m_transform;

    private class InteractionTracer
    {
        private Type interactorType;
        private List<Interactor> interacting = new();

        public InteractionTracer(Type t) {
            interactorType = t;
        }

        public void UpdateInteraction(Transform m_transform, float interactRadius, InteractableObject obj)
        {
            // Prevent interaction from happening for objects out of range
            for (int i = interacting.Count - 1; i >= 0; i--)
            {
                Interactor interactor = interacting[i];

                if (!InteractionAttribute.ExistInteractor(interactor)) {
                    interacting.RemoveAt(i);
                    Debug.Log("Removing disabled interactor");
                    continue;
                }

                Transform t = InteractionAttribute.GetInteractorTransform(interactor);
                if (Vector2.Distance(t.position, m_transform.position) > interactRadius)
                {
                    interacting.RemoveAt(i);
                    interactor.RemoveInteractable(obj);
                }
            }

            // Establish connection for objects in range
            Interactor[] interactors = InteractionAttribute.GetInteractors(interactorType);
            if (interactors == null)
            {
                return;
            }

            foreach (Interactor interactor in interactors) {
                if (interacting.Contains(interactor)) {
                    continue;
                }
                Transform t = InteractionAttribute.GetInteractorTransform(interactor);
                if (Vector2.Distance(t.position, m_transform.position) <= interactRadius)
                {
                    interacting.Add(interactor);
                    interactor.AddInteractable(obj);
                }
            }
        }

        public void StopInteractions(InteractableObject obj) {
            for (int j = interacting.Count - 1; j >= 0; j--) {
                Interactor i = interacting[j];
                i.RemoveInteractable(obj);
                interacting.Remove(i);
            }
        }
    }

    // Call this method in child classes using "base.Start()" to avoid unexpected behaviors
    public void Start()
    {
        Type[] arr = InteractionAttribute.GetInteractorTypes(GetType());
        if (arr == null) {
            return;
        }
        foreach (Type t in arr)
        {
            interactionTracers[t] = new InteractionTracer(t);
        }
        m_transform = GetComponent<Transform>();
    }

    public virtual string Interact(Interactor interactor, InteractionType interactType){ return Setting.INTERACTION_OK; }

    public abstract Dictionary<InteractionType, string> GetInteractionType(Type t);

    public void Update()
    {
        foreach (InteractionTracer tracer in interactionTracers.Values)
        {
            tracer.UpdateInteraction(m_transform, interactRadius.Value, this);
        }
    }

    public void OnDisable()
    {
        foreach (InteractionTracer tracer in interactionTracers.Values) { 
            tracer.StopInteractions(this);
        }
    }
}
