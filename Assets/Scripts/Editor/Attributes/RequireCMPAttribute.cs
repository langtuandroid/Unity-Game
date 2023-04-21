using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireCMPAttribute : Attribute
    {
        public static Dictionary<Type, HashSet<Type>> requirement = new();
        public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

        public RequireCMPAttribute(Type ability, params Type[] requiredComponents)
        {
            if (!ability.IsSubclassOf(typeof(Ability)))
            {
                Debug.LogError("Type:" + ability.ToString() + " is not an Ability!");
                return;
            }
            if (requirement.ContainsKey(ability))
            {
                Debug.LogWarning("Ability:" + ability.ToString() + " is requiring components from multiple sources!");
                return;
            }
            requirement[ability] = new HashSet<Type>();
            foreach (Type t in requiredComponents)
            {
                if (!t.IsSubclassOf(typeof(Component)))
                {
                    Debug.LogError("Cannot apply require Component of type:" + t.ToString() + " to " + ability.ToString());
                    return;
                }
                requirement[ability].Add(t);

                if (!rev_requirement.ContainsKey(t))
                {
                    rev_requirement[t] = new HashSet<Type>();
                }
                rev_requirement[t].Add(ability);
            }
        }
    }
}
