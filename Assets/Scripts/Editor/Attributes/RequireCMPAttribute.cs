using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class RequireCMPAttribute : Attribute
{
    public static Dictionary<Type, HashSet<Type>> requirement = new();
    public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

    public RequireCMPAttribute(Type actionInstance, params Type[] requiredComponents)
    {
        if (!actionInstance.IsSubclassOf(typeof(ActionInstance)))
        {
            Debug.LogError("Type:" + actionInstance.ToString() + " is not an ActionInstance!");
            return;
        }
        if (requirement.ContainsKey(actionInstance))
        {
            Debug.LogWarning("ActionInstance:" + actionInstance.ToString() + " is requiring components from multiple sources!");
            return;
        }
        requirement[actionInstance] = new HashSet<Type>();
        foreach (Type t in requiredComponents)
        {
            if (!t.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError("Cannot apply require Component of type:" + t.ToString() + " to " + actionInstance.ToString());
                return;
            }
            requirement[actionInstance].Add(t);

            if (!rev_requirement.ContainsKey(t)) {
                rev_requirement[t] = new HashSet<Type>();
            }
            rev_requirement[t].Add(actionInstance);
        }
    }
}
