using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireWeaponStatAttribute : Attribute
    {
        public static Dictionary<Type, HashSet<Type>> typeRequirements = new();
        private List<Type> weaponStatTypes;

        public RequireWeaponStatAttribute(params Type[] weaponStats) {
            weaponStatTypes = new();
            foreach (Type type in weaponStats) {
                if (type.IsSubclassOf(typeof(WeaponStat)))
                {
                    weaponStatTypes.Add(type);
                }
                else {
                    Debug.LogWarning("Attempting to add " + type.FullName + " to weapon stat requirement which is not a valid weapon stat type.");
                }
            }
        }

        public void Init(Type type) {
            if (!typeRequirements.ContainsKey(type)) {
                typeRequirements.Add(type, new HashSet<Type>());
            }
            foreach (Type t in weaponStatTypes) {
                typeRequirements[t].Add(type);
            }
        }
    }
}
