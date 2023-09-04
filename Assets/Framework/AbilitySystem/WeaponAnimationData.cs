using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility;
using System;

namespace LobsterFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "Ability/WeaponAnimationData")]
    public class WeaponAnimationData : ScriptableObject
    {
        [SerializeField] internal List<WeaponAbilityAnimationSetting> setting;
        [SerializeField] internal List<AnimationClip> moveSetting;

        public AnimationClip GetAbilityClip(WeaponType weaponType, Type abilityType) {
            WeaponAbilityAnimationSetting st = setting[(int)weaponType];
            string ability = abilityType.AssemblyQualifiedName;
            if (st.ContainsKey(ability)) {
                return st[ability];
            }
            return null;
        }

        public AnimationClip GetMoveClip(WeaponType weaponType) {
            return moveSetting[(int)weaponType];
        }
    }
}
