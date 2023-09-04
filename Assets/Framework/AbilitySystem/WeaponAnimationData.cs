using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "Ability/WeaponAnimationData")]
    public class WeaponAnimationData : ScriptableObject
    {
        [SerializeField] internal WeaponAnimationDictionary setting;

        public AnimationClip GetClip<T>(WeaponType weaponType) {
            if (setting.ContainsKey(weaponType)) {
                WeaponAbilityAnimationSetting st = setting[weaponType];
                string ability = typeof(T).AssemblyQualifiedName;
                if (st.ContainsKey(ability)) {
                    return st[ability];
                }
                return null;
            }
            return null;
        }
    }
}
