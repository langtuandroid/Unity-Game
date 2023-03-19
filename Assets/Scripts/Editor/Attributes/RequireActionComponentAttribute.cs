using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.Action
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireActionComponentAttribute : Attribute
    {
        public static Dictionary<Type, HashSet<Type>> requirement = new();
        public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

        public RequireActionComponentAttribute(Type actionInstance, params Type[] actionComponents)
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
            foreach (Type t in actionComponents)
            {
                if (!t.IsSubclassOf(typeof(ActionComponent)))
                {
                    Debug.LogError("Cannot apply require ActionComponent of type:" + t.ToString() + " to " + actionInstance.ToString());
                    return;
                }
                requirement[actionInstance].Add(t);

                if (!rev_requirement.ContainsKey(t))
                {
                    rev_requirement[t] = new HashSet<Type>();
                }
                rev_requirement[t].Add(actionInstance);
            }
        }
    }
}
