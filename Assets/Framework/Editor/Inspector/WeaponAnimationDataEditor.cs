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
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            selectedWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", selectedWeaponType);
            WeaponAnimationData data = (WeaponAnimationData)target;
            Undo.RecordObject(data, "WeaponAnimationData");
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
                for (int i = data.setting.Count; i <= enums.Length; i++)
                {
                    data.moveSetting.Add(null);
                }
            }

            WeaponAbilityAnimationSetting setting = data.setting[(int)selectedWeaponType];
            List<Type> displayEntries = new();
            if (selectedWeaponType != WeaponType.EmptyHand) {
                displayEntries.Add(typeof(LightWeaponAttack));
                displayEntries.Add(typeof(HeavyWeaponAttack));
                displayEntries.Add(typeof(Guard));
            }

            if (AddWeaponArtMenuAttribute.compatibleAbilities.ContainsKey(selectedWeaponType)) {
                HashSet<Type> collection = AddWeaponArtMenuAttribute.compatibleAbilities[selectedWeaponType];
                displayEntries.AddRange(collection);
            }

            foreach (Type ability in displayEntries) {
                if (!setting.ContainsKey(ability.AssemblyQualifiedName)) {
                    setting[ability.AssemblyQualifiedName] = null;
                }
                setting[ability.AssemblyQualifiedName] = (AnimationClip)EditorGUILayout.ObjectField(ability.Name, setting[ability.AssemblyQualifiedName], typeof(AnimationClip), false);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            int selected = (int)selectedWeaponType;
            data.moveSetting[selected] = (AnimationClip)EditorGUILayout.ObjectField("Move", data.moveSetting[selected], typeof(AnimationClip), false);
            if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
