using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireWeaponAttribute : Attribute
    {
        private static Dictionary<string, HashSet<WeaponType>> weaponRequirements = new();

        public RequireWeaponAttribute(Type abilityType, WeaponType weaponType) {
            if (!abilityType.IsSubclassOf(typeof(Ability))){
                Debug.LogError("The ability type specified is not a subclass of Ability: " + abilityType.Name);
                return;
            }
            if (!weaponRequirements.ContainsKey(abilityType.Name)) { 
                weaponRequirements.Add(abilityType.Name, new HashSet<WeaponType>());
            }
            weaponRequirements[abilityType.Name].Add(weaponType);
        }
    }
}
