using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class InteractableObject : MonoBehaviour
{
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

    public void Start()
    {
        Type[] arr = InteractionAttribute.GetInteractorTypes(GetType());
        foreach (Type t in arr)
        {
            interactionTracers[t] = new InteractionTracer(t);
        }
        m_transform = GetComponent<Transform>();
    }

    public virtual void Interact(Interactor interactor) { }

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
