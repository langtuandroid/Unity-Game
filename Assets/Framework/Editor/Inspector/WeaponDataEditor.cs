using LobsterFramework.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(WeaponData))]
    public class WeaponDataEditor : Editor
    {
        private Dictionary<Type, Editor> weaponStatsEditors = new();

        private GUIStyle selectWeaponStatStyle = new();

        public WeaponStat selectedWeaponStat = null;

        private Rect addWeaponStatRect;
        private Rect selectWeaponStatRect;

        public void OnEnable()
        {
            selectWeaponStatStyle.fontStyle = FontStyle.Bold;
            selectWeaponStatStyle.normal.textColor = Color.cyan;
            selectWeaponStatStyle.hover.background = Texture2D.grayTexture;
        }

        public override void OnInspectorGUI()
        {
            WeaponData weaponData = (WeaponData)target;
            EditorGUI.BeginChangeCheck();

            try
            {
                DrawWeaponStats(weaponData);
            }
            catch (ArgumentException)
            {
                // Ignore this error
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawWeaponStats(WeaponData weaponData)
        {
            SerializedProperty weaponStats = serializedObject.FindProperty("weaponStats");
            EditorGUILayout.BeginHorizontal();
            weaponStats.isExpanded = EditorGUILayout.Foldout(weaponStats.isExpanded, "Weapon Stats: " + weaponData.weaponStats.Values.Count);
            GUILayout.FlexibleSpace();
            bool aButton = GUILayout.Button("Add Stat", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            if (aButton) // Add action component button clicked
            {
                AddWeaponStatPopup popup = new();
                popup.data = weaponData;
                PopupWindow.Show(addWeaponStatRect, popup);
            }

            if (weaponStats.isExpanded)
            {
                if (weaponData.weaponStats.Count == 0)
                {
                    EditorGUILayout.LabelField("No ability stats available for display!");
                }
                else
                {
                    EditorGUILayout.Space();
                    if (selectedWeaponStat == null)
                    {
                        selectedWeaponStat = weaponData.weaponStats.First().Value;
                    }
                    Editor editor;
                    Type type = selectedWeaponStat.GetType();
                    if (weaponStatsEditors.ContainsKey(type))
                    {
                        editor = weaponStatsEditors[type];
                    }
                    else
                    {
                        editor = CreateEditor(selectedWeaponStat);
                        weaponStatsEditors.Add(type, editor);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUIContent content = new();
                    bool selected;
                    content.text = selectedWeaponStat.GetType().Name;
                    if (AddWeaponStatMenuAttribute.icons.ContainsKey(type))
                    {
                        content.image = AddWeaponStatMenuAttribute.icons[type];
                        selected = GUILayout.Button(content, selectWeaponStatStyle, GUILayout.Height(40));
                    }
                    else
                    {
                        selected = GUILayout.Button(content, selectWeaponStatStyle);
                    }

                    if (selected)
                    {
                        SelectWeaponStatPopup popup = new SelectWeaponStatPopup();
                        popup.editor = this;
                        popup.data = weaponData;
                        PopupWindow.Show(selectWeaponStatRect, popup);
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    bool clicked = EditorUtils.Button(Color.red, "Remove Stat", GUILayout.Width(100));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                    if (!clicked)
                    {
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        var m = typeof(WeaponData).GetMethod("RemoveWeaponStat", BindingFlags.Instance | BindingFlags.NonPublic);
                        MethodInfo removed = m.MakeGenericMethod(selectedWeaponStat.GetType());
                        weaponStatsEditors.Remove(type);
                        removed.Invoke(weaponData, null);
                    }
                }
            }
        }
    }
}
