using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;
using System;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(WeaponAnimationData))]
    public class WeaponAnimationDataEditor : Editor
    {
        private WeaponType selectedWeaponType;

        public WeaponAnimationDataEditor() {
            selectedWeaponType = WeaponType.Sword;
        }

        public override void OnInspectorGUI()
        {
            selectedWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", selectedWeaponType);
            WeaponAnimationData data = (WeaponAnimationData)target;
            if (!data.setting.ContainsKey(selectedWeaponType)) {
                data.setting[selectedWeaponType] = new();
            }

            WeaponAbilityAnimationSetting setting = data.setting[selectedWeaponType];
            List<Type> displayEntries = new();
            if (selectedWeaponType != WeaponType.EmptyHand) {
                displayEntries.Add(typeof(LightWeaponAttack));
                displayEntries.Add(typeof(HeavyWeaponAttack));
            }

            if (AddWeaponArtMenuAttribute.compatibleAbilities.ContainsKey(selectedWeaponType)) {
                HashSet<Type> collection = AddWeaponArtMenuAttribute.compatibleAbilities[selectedWeaponType];
                displayEntries.AddRange(collection);
            }

            if (displayEntries.Count == 0) {
                EditorGUILayout.LabelField("No ability animation available for this weapon!");
                return;
            }
            foreach (Type ability in displayEntries) {
                if (!setting.ContainsKey(ability.AssemblyQualifiedName)) {
                    setting[ability.AssemblyQualifiedName] = null;
                }
                setting[ability.AssemblyQualifiedName] = (AnimationClip)EditorGUILayout.ObjectField(ability.Name, setting[ability.AssemblyQualifiedName], typeof(AnimationClip), false);
            }
        }
    }
}
