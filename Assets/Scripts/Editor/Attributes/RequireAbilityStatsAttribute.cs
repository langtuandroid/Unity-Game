using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// Marks this ability as requiring specified AbilityStats to run
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireAbilityStatsAttribute : Attribute
    {
        public static Dictionary<Type, HashSet<Type>> requirement = new();
        public static Dictionary<Type, HashSet<Type>> rev_requirement = new();

        private Type[] abilityStats;

        public RequireAbilityStatsAttribute(Type ability, params Type[] abilityStats)
        {
            this.abilityStats = abilityStats;
        }

        public void Init(Type ability) {
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
            foreach (Type t in abilityStats)
            {
                if (!t.IsSubclassOf(typeof(AbilityStat)))
                {
                    Debug.LogError("Cannot apply require AbilityStat of type:" + t.ToString() + " to " + ability.ToString());
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
