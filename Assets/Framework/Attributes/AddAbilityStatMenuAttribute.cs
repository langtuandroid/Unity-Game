using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddAbilityStatMenuAttribute : Attribute
    {
        public static HashSet<Type> types = new HashSet<Type>();

        public void Init(Type type) {
            if (type.IsSubclassOf(typeof(AbilityStat)))
            {
                types.Add(type);
            }
            else
            {
                Debug.LogError("Attempting to apply AbilitStat Attribute on invalid type: " + type.Name);
            }
        }
    }
}
