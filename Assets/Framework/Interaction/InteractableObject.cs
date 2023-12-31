using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.Interaction
{
    public enum InteractionType
    {
        Primary,
        Secondary,
        Tertiary,
        Quaternary
    }

    public struct InteractionPrompt
    {
        public static InteractionPrompt none = new InteractionPrompt() { };

        public string Primary { get; set; }
        public string Secondary { get; set; }
        public string Tertiary { get; set; }
        public string Quaternary { get; set; }
    }

    public abstract class InteractableObject : MonoBehaviour
    {
        /* Base Class for interactable objects, inherit from this class to implement custom interaction behaviors.
         * 
         * Note: The derived class must call base.Start() to allow the base class to complete all the necessary setups
         */
        [SerializeField] protected internal float interactRadius;
        [SerializeField] protected internal bool interactEnabled;
        [SerializeField] protected StringEventChannel interactionChannel;
        private Dictionary<Type, InteractionTracer> interactionTracers = new();
        private Transform m_transform;

        // Call this method in child classes using "base.Start()" to avoid unexpected behaviors
        public void Start()
        {
            Type[] arr = InteractionAttribute.GetInteractorTypes(GetType());
            if (arr == null)
            {
                return;
            }
            foreach (Type t in arr)
            {
                interactionTracers[t] = new InteractionTracer(t);
            }
            m_transform = GetComponent<Transform>();
            if (interactRadius == 0) {
                interactRadius = 1;
            }
        }

        public bool InteractEnabled
        {
            get { return interactEnabled; }
            set
            {
                interactEnabled = value;
                if (!interactEnabled)
                {
                    foreach (InteractionTracer tracer in interactionTracers.Values)
                    {
                        tracer.StopInteractions(this);
                    }
                }
            }
        }

        public virtual void OnInteract(Interactor interactor, InteractionType interactType) { }

        public abstract InteractionPrompt GetInteractionOptions(Type t);

        public void Update()
        {
            if (!interactEnabled)
            {
                return;
            }
            foreach (InteractionTracer tracer in interactionTracers.Values)
            {
                tracer.UpdateInteraction(m_transform, interactRadius, this);
            }
            IUpdate();
        }

        protected virtual void IUpdate() { }

        public void OnDisable()
        {
            foreach (InteractionTracer tracer in interactionTracers.Values)
            {
                tracer.StopInteractions(this);
            }
        }

        // Class used for managing existing interactions as well as discovering new interactors available for interactions
        private class InteractionTracer
        {
            private Type interactorType;
            private List<Interactor> interacting = new();
            private List<Interactor> interactors = new();

            public InteractionTracer(Type t)
            {
                interactorType = t;
            }

            public void UpdateInteraction(Transform m_transform, float interactRadius, InteractableObject obj)
            {
                // Prevent interaction from happening for objects out of range
                for (int i = interacting.Count - 1; i >= 0; i--)
                {
                    Interactor interactor = interacting[i];

                    if (!InteractionAttribute.ExistInteractor(interactor))
                    {
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
                InteractionAttribute.GetInteractors(interactorType, interactors);
                if (interactors.Count == 0)
                {
                    return;
                }

                foreach (Interactor interactor in interactors)
                {
                    if (interacting.Contains(interactor))
                    {
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

            public void StopInteractions(InteractableObject obj)
            {
                for (int j = interacting.Count - 1; j >= 0; j--)
                {
                    Interactor i = interacting[j];
                    i.RemoveInteractable(obj);
                    interacting.Remove(i);
                }
            }
        }
    }
}
