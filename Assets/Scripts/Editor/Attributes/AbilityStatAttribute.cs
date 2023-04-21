using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AbilityStatAttribute : Attribute
    {
        public static HashSet<Type> types = new HashSet<Type>();
        public AbilityStatAttribute(Type type)
        {
            if (type.IsSubclassOf(typeof(AbilityStat)))
            {
                types.Add(type);
            }
            else {
                Debug.LogError("Attempting to apply AbilitStat Attribute on invalid type: " + type.Name);
            }
        }
    }
}
