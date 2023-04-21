using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireAbilityStatsAttribute : Attribute
    {
        public static Dictionary<Type, HashSet<Type>> requirement = new();
        public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

        public RequireAbilityStatsAttribute(Type ability, params Type[] actionStats)
        {
            if (!ability.IsSubclassOf(typeof(Ability)))
            {
                Debug.LogError("Type:" + ability.ToString() + " is not an ActionInstance!");
                return;
            }
            if (requirement.ContainsKey(ability))
            {
                Debug.LogWarning("ActionInstance:" + ability.ToString() + " is requiring components from multiple sources!");
                return;
            }
            requirement[ability] = new HashSet<Type>();
            foreach (Type t in actionStats)
            {
                if (!t.IsSubclassOf(typeof(AbilityStat)))
                {
                    Debug.LogError("Cannot apply require ActionComponent of type:" + t.ToString() + " to " + ability.ToString());
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
