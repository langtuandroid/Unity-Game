using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace LobsterFramework.Interaction
{
    /// <summary>
    /// Specifies the types of interactors this interable object can have interactions with.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InteractionAttribute : Attribute
    {
        private static Dictionary<Type, HashSet<Type>> interactorTypes = new();
        private static Dictionary<Type, HashSet<Interactor>> allInteractors = new();
        private static Dictionary<Interactor, Transform> transformBuffer = new();

        private Type[] interactors;

        public InteractionAttribute(params Type[] interactors)
        {
            this.interactors = interactors;   
        }

        public void Init(Type interactable) {
            if (!interactable.IsSubclassOf(typeof(InteractableObject)))
            {
                Debug.LogError(typeof(InteractableObject).ToString() + " is not a subtype of Interactable Object!");
                return;
            }
            interactorTypes[interactable] = new();
            if (interactors.Length == 0)
            {
                Debug.Log("Missing arguments for interactor types!");
            }
            foreach (Type t in interactors)
            {
                if (!t.IsSubclassOf(typeof(Interactor)))
                {
                    Debug.LogError("Cannot add type: " + t.ToString() + " as the interactor for " + interactable.ToString());
                    continue;
                }
                interactorTypes[interactable].Add(t);
            }
        }

        public static Type[] GetInteractorTypes(Type interactable)
        {
            if (interactorTypes.ContainsKey(interactable))
            {
                Type[] arr = new Type[interactorTypes[interactable].Count];
                interactorTypes[interactable].CopyTo(arr);
                return arr;
            }
            return null;
        }

        public static void GetInteractors(Type interactorType, List<Interactor> container)
        {
            container.Clear();
            if (allInteractors.ContainsKey(interactorType))
            {
                HashSet<Interactor> i = allInteractors[interactorType];
                foreach (Interactor intc in i)
                {
                    container.Add(intc);
                }
                return;
            }
        }

        public static Transform GetInteractorTransform(Interactor interactor)
        {
            if (transformBuffer.ContainsKey(interactor))
            {
                return transformBuffer[interactor];
            }
            return null;
        }

        public static bool IsInteractable(Type interactable, Type interactor)
        {
            if (!interactorTypes.ContainsKey(interactable))
            {
                return false;
            }
            return interactorTypes[interactable].Contains(interactor);
        }

        public static void AddInteractor(Interactor interactor)
        {
            Type t = interactor.GetType();
            if (!allInteractors.ContainsKey(t))
            {
                allInteractors[t] = new();
            }
            allInteractors[t].Add(interactor);
            transformBuffer[interactor] = interactor.GetComponent<Transform>();
        }

        public static void RemoveInteractor(Interactor interactor)
        {
            Type t = interactor.GetType();
            if (allInteractors.ContainsKey(t))
            {
                allInteractors[t].Remove(interactor);
                transformBuffer.Remove(interactor);
            }
        }

        public static bool ExistInteractor(Interactor interactor)
        {
            Type t = interactor.GetType();
            if (allInteractors.ContainsKey(t))
            {
                return allInteractors[t].Contains(interactor);
            }
            return false;
        }
    }
}
