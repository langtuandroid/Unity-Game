using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;
using System;
using UnityEditor.Playables;

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
            serializedObject.Update();
            selectedWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", selectedWeaponType);
            WeaponAnimationData data = (WeaponAnimationData)target;
            Array enums = Enum.GetValues(typeof(WeaponType));
            if (data.setting == null) {
                data.setting = new();
            }
            if (data.moveSetting == null) {
                data.moveSetting = new();
            }
            if (data.setting.Count < enums.Length) {
                for (int i = data.setting.Count;i <= enums.Length;i++) {
                    data.setting.Add(new());
                }
            }
            if (data.moveSetting.Count < enums.Length) {
                for (int i = data.moveSetting.Count; i <= enums.Length; i++)
                {
                    data.moveSetting.Add(null);
                }
            }

            AbilityAnimationConfig setting = data.setting[(int)selectedWeaponType];
            List<Type> displayEntries = new();
            displayEntries.Add(typeof(LightWeaponAttack));
            displayEntries.Add(typeof(HeavyWeaponAttack));
            displayEntries.Add(typeof(Guard));

            if (AddWeaponArtMenuAttribute.compatibleAbilities.ContainsKey(selectedWeaponType)) {
                HashSet<Type> collection = AddWeaponArtMenuAttribute.compatibleAbilities[selectedWeaponType];
                displayEntries.AddRange(collection);
            }

            foreach (Type ability in displayEntries) {
                if (!setting.ContainsKey(ability.AssemblyQualifiedName)) { 
                    setting[ability.AssemblyQualifiedName] = null;
                }
                DisplayAbilityAnimationEntries(setting, ability);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            int selected = (int)selectedWeaponType;
            data.moveSetting[selected] = (AnimationClip)EditorGUILayout.ObjectField("Move", data.moveSetting[selected], typeof(AnimationClip), false);
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayAbilityAnimationEntries(AbilityAnimationConfig setting, Type abilityType) {
            AnimationClip[] clips = setting[abilityType.AssemblyQualifiedName];
            if (!WeaponAnimationAttribute.abilityAnimationEntry.ContainsKey(abilityType))
            {
                if (clips == null) {
                    setting[abilityType.AssemblyQualifiedName] = new AnimationClip[1];
                    clips = setting[abilityType.AssemblyQualifiedName];
                }
                clips[0] = (AnimationClip)EditorGUILayout.ObjectField(abilityType.Name, clips[0], typeof(AnimationClip), false);
            }
            else {
                EditorGUILayout.LabelField(abilityType.Name);
                EditorGUI.indentLevel++;
                string[] enums = Enum.GetNames(WeaponAnimationAttribute.abilityAnimationEntry[abilityType]);
                if (clips == null)
                {
                    setting[abilityType.AssemblyQualifiedName] = new AnimationClip[enums.Length];
                    clips = setting[abilityType.AssemblyQualifiedName];
                }
                for (int i = 0;i < enums.Length;i++) {
                    clips[i] = (AnimationClip)EditorGUILayout.ObjectField(enums[i], clips[i], typeof(AnimationClip), false);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
