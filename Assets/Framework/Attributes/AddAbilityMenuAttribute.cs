using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem {

    /// <summary>
    /// Used to add Abilities to the pool of available Abilities. This will allow the creations of these abilities inside AbilityData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddAbilityMenuAttribute : Attribute
    {
        public static HashSet<Type> actions = new HashSet<Type>();

        public void AddAbility(Type type) {
            if (type.IsSubclassOf(typeof(Ability)))
            {
                actions.Add(type);
            }
            else {
                Debug.LogError("The type specified for ability menu is not an ability:" + type.Name);
            }
        }
    }
}
