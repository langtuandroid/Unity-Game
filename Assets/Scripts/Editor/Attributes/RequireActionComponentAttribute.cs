using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class RequireActionComponentAttribute : Attribute
{
    public static Dictionary<Type, HashSet<Type>> requirement = new();
    public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

    public RequireActionComponentAttribute(Type e, params Type[] ts) {
        if (!e.IsSubclassOf(typeof(ActionInstance)))
        {
            Debug.LogError("Type:" + e.ToString() + " is not an ActionInstance!");
            return;
        }
        if (requirement.ContainsKey(e))
        {
            Debug.LogWarning("ActionInstance:" + e.ToString() + " is requiring components from multiple sources!");
            return;
        }
        requirement[e] = new HashSet<Type>();
        foreach (Type t in ts) {
            if (!t.IsSubclassOf(typeof(ActionComponent)))
            {
                Debug.LogError("Cannot apply require ActionComponent of type:" + t.ToString() + " to " + e.ToString());
                return;
            }
            requirement[e].Add(t);

            if (!rev_requirement.ContainsKey(t))
            {
                rev_requirement[t] = new HashSet<Type>();
            }
            rev_requirement[t].Add(e);
        }
    }
}
