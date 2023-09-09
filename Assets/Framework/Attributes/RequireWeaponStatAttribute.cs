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
        private static Dictionary<Type, Tuple<bool, HashSet<Type>>> typeRequirements = new();
        private List<Type> weaponStatTypes;
        private bool isMainhand;

        public RequireWeaponStatAttribute(bool isMainhand, params Type[] weaponStats) {
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
            this.isMainhand = isMainhand;
        }

        public void Init(Type type) {
            if (!typeRequirements.ContainsKey(type)) {
                typeRequirements.Add(type, new(isMainhand, new()));
            }
            foreach (Type t in weaponStatTypes) {
                typeRequirements[t].Item2.Add(type);
            }
        }

        /// <summary>
        /// Check to see if the weapon contains all the WeaponStats required by the ability
        /// </summary>
        /// <param name="abilityType">The type of the ability being queried</param>
        /// <param name="weapon">The weapon being queried</param>
        /// <returns>True if the weapon contains all of the required stats, otherwise false</returns>
        public static bool HasWeaponStats(Type abilityType, WeaponWielder weaponWielder) {
            if (!abilityType.IsSubclassOf(typeof(WeaponAbility))) {
                Debug.LogWarning("The ability type being queried is not a WeaponAbility!");
                return false;
            }
            if (weaponWielder == null) {
                return false;
            }

            if (typeRequirements.ContainsKey(abilityType)) {
                (bool isMainhand, HashSet<Type> requirements) = typeRequirements[abilityType];
                Weapon querying;
                if (isMainhand) {
                    querying = weaponWielder.Mainhand;
                }
                else {
                    querying = weaponWielder.Offhand;
                }
                if (querying == null)
                {
                    return false;
                }

                foreach (Type type in requirements) {
                    if (!querying.HasWeaponStat(type)) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
