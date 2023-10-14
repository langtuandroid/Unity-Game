using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework
{
    /// <summary>
    /// Mark the WeaponAbility as an offhand WeaponAbility
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OffhandWeaponAbilityAttribute : Attribute
    {
        private static readonly HashSet<Type> abilityTypes = new();

        public static bool IsOffhand(Type type)
        {
            return abilityTypes.Contains(type);
        }

        public void Init(Type abilityType) {
            if (abilityType.IsSubclassOf(typeof(WeaponAbility))) { 
                abilityTypes.Add(abilityType);
            }
        }
    }
}
