using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class InteractionAttribute : Attribute
{
    private static Dictionary<Type, HashSet<Type>> interactorTypes = new();
    private static Dictionary<Type, HashSet<Interactor>> interactors = new();
    private static Dictionary<Interactor, Transform> transformBuffer = new();

    public InteractionAttribute(Type interactable, params Type[] interactors)
    {
        if (!interactable.IsSubclassOf(typeof(InteractableObject))) {
            Debug.LogError(typeof(InteractableObject).ToString() + " is not an Interactable Object!");
            return;
        }
        interactorTypes[interactable] = new();
        if (interactors.Length == 0) {
            Debug.Log("Missing arguments for interactor types!");
        }
        foreach (Type t in interactors) {
            if (!t.IsSubclassOf(typeof(Interactor))) {
                Debug.LogError("Cannot add type: " + t.ToString() + " as the interactor for " + interactable.ToString());
                continue;
            }
            interactorTypes[interactable].Add(t);
        }
    }

    public static Type[] GetInteractorTypes(Type interactable) {
        if (interactorTypes.ContainsKey(interactable)) {
            Type[] arr = new Type[interactorTypes[interactable].Count];
            interactorTypes[interactable].CopyTo(arr);
            return arr;
        }
        return null;
    }

    public static Interactor[] GetInteractors(Type interactorType) {
        
        if (interactors.ContainsKey(interactorType)) {
            HashSet<Interactor> i = interactors[interactorType];
            if (i.Count == 0) {
                return null;
            }
            Interactor[] ins = new Interactor[i.Count];
            interactors[interactorType].CopyTo(ins);
            return ins;
        }
        return null;
    }

    public static Transform GetInteractorTransform(Interactor interactor) {
        if (transformBuffer.ContainsKey(interactor)) {
            return transformBuffer[interactor];
        }
        return null;
    }

    public static bool IsInteractable(Type interactable, Type interactor)
    {
        if (!interactorTypes.ContainsKey(interactable)) {
            return false;
        }
        return interactorTypes[interactable].Contains(interactor);
    }

    public static void AddInteractor(Interactor interactor) {
        Type t = interactor.GetType();
        if (!interactors.ContainsKey(t)) {
            interactors[t] = new();
        }
        interactors[t].Add(interactor);
        transformBuffer[interactor] = interactor.GetComponent<Transform>();
    }

    public static void RemoveInteractor(Interactor interactor)
    {
        Type t = interactor.GetType();
        if (interactors.ContainsKey(t))
        {
            interactors[t].Remove(interactor);
            transformBuffer.Remove(interactor);
        }
    }

    public static bool ExistInteractor(Interactor interactor) {
        Type t = interactor.GetType();
        if (interactors.ContainsKey(t)) {
            return interactors[t].Contains(interactor);
        }
        return false;  
    }
}
