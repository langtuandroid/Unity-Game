using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using LobsterFramework.AbilitySystem;
using System.Linq;
using System.Threading;
using GluonGui.Dialog;
using static LobsterFramework.AbilitySystem.Ability;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(AbilityData))]
    public class AbilityDataEditor : Editor
    {
        private Dictionary<Type, Editor> abilityEditors = new();
        private Dictionary<Type, Editor> abilityStatsEditors = new();
        private GUIStyle style1 = new();
        private GUIStyle style2 = new();

        private GUIStyle selectAbilityStyle = new();
        private GUIStyle selectAbilityStatStyle = new();

        public Ability selectedAbility = null;
        public AbilityStat selectedAbilityStat = null;

        private Rect addAbilityRect;
        private Rect selectAbilityRect;
        private Rect addAbilityStatRect;
        private Rect selectAbilityStatRect;

        public void OnEnable()
        {
            style1.normal.textColor = Color.cyan;
            style1.fontStyle = FontStyle.Bold;
            style2.normal.textColor = Color.yellow;
            style2.fontStyle = FontStyle.Bold;

            selectAbilityStyle.fontStyle = FontStyle.Bold;
            selectAbilityStyle.normal.textColor = Color.yellow;
            selectAbilityStyle.hover.background = Texture2D.grayTexture;

            selectAbilityStatStyle.fontStyle = FontStyle.Bold;
            selectAbilityStatStyle.normal.textColor = Color.cyan;
            selectAbilityStatStyle.hover.background = Texture2D.grayTexture;
        }

        public override void OnInspectorGUI()
        {
            AbilityData abilityData = (AbilityData)target;

            EditorGUILayout.HelpBox("Note: The configurations for abilities will not be available before " +
                "the first run of the game! Please verify the integrity of data " +
                "by running the game first before moving on to remove corrupted configs.", MessageType.Info);
            EditorGUI.BeginChangeCheck();

            DrawAbilityStats(abilityData);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            DrawAbilities(abilityData);
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }
        }

        private void DrawAbilityStats(AbilityData abilityData) {
            SerializedProperty abilityStats = serializedObject.FindProperty("stats");
            EditorGUILayout.BeginHorizontal();
            abilityStats.isExpanded = EditorGUILayout.Foldout(abilityStats.isExpanded, "Ability Stats: " + abilityData.stats.Values.Count);
            GUILayout.FlexibleSpace();
            bool aButton = GUILayout.Button("Add AbilityStat", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            if (aButton) // Add action component button clicked
            {
                AddAbilityStatPopup popup = new();
                popup.data = abilityData;
                PopupWindow.Show(addAbilityStatRect, popup);
            }

            if (abilityStats.isExpanded)
            {
                if (abilityData.stats.Count == 0)
                {
                    EditorGUILayout.LabelField("No ability stats available for display!");
                }
                else
                {
                    EditorGUILayout.Space();
                    if (selectedAbilityStat == null)
                    {
                        selectedAbilityStat = abilityData.stats.First().Value;
                    }
                    Editor editor;
                    Type type = selectedAbilityStat.GetType();
                    if (abilityStatsEditors.ContainsKey(type))
                    {
                        editor = abilityStatsEditors[type];
                    }
                    else
                    {
                        editor = CreateEditor(selectedAbilityStat);
                        abilityStatsEditors.Add(type, editor);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUIContent content = new();
                    bool selected;
                    content.text = selectedAbilityStat.GetType().Name;
                    if (AddAbilityStatMenuAttribute.icons.ContainsKey(type))
                    {
                        content.image = AddAbilityStatMenuAttribute.icons[type];
                        selected = GUILayout.Button(content, selectAbilityStatStyle, GUILayout.Height(40));
                    }
                    else
                    {
                        selected = GUILayout.Button(content, selectAbilityStatStyle);
                    }

                    if (selected)
                    {
                        SelectAbilityStatPopup popup = new SelectAbilityStatPopup();
                        popup.editor = this;
                        popup.data = abilityData;
                        PopupWindow.Show(selectAbilityStatRect, popup);
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
                        var m = typeof(AbilityData).GetMethod("RemoveAbilityStat", BindingFlags.Instance | BindingFlags.NonPublic);
                        MethodInfo removed = m.MakeGenericMethod(selectedAbilityStat.GetType());
                        abilityStatsEditors.Remove(type);
                        removed.Invoke(abilityData, null);
                    }
                }
            }
        }

        private void DrawAbilities(AbilityData abilityData) {
            // Draw Ability Section
            SerializedProperty abilities = serializedObject.FindProperty("allAbilities");

            EditorGUILayout.BeginHorizontal();
            abilities.isExpanded = EditorGUILayout.Foldout(abilities.isExpanded, "Abilities: " + abilityData.allAbilities.Count);
            GUILayout.FlexibleSpace();
            bool aiButton = GUILayout.Button("Add Ability", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            if (aiButton) // Add ability button clicked 
            {
                AddAbilityPopup.data = abilityData;
                AddAbilityPopup popup = new AddAbilityPopup();
                PopupWindow.Show(addAbilityRect, popup);
            }

            if (abilities.isExpanded)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Note: When editing list properties of abilities, drag reference directly to the list itself instead of its element fields, " +
                "otherwise the reference may not be saved.", MessageType.Info, true);
                UnityEditor.Editor editor;
                if (abilityData.allAbilities.Count == 0)
                {
                    EditorGUILayout.LabelField("No abilities available for display!");
                }
                else
                {
                    if (selectedAbility == null)
                    {
                        selectedAbility = abilityData.allAbilities.First().Value;
                    }
                    Type abilityType = selectedAbility.GetType();
                    if (!abilityEditors.ContainsKey(abilityType))
                    {
                        editor = CreateEditor(selectedAbility);
                        abilityEditors.Add(abilityType, editor);
                    }
                    else
                    {
                        editor = abilityEditors[abilityType];
                    }

                    #region DrawAbilityEditorSection
                    EditorGUILayout.BeginHorizontal();

                    GUIContent content = new();
                    bool selected;
                    content.text = selectedAbility.GetType().Name;
                    if (AddAbilityMenuAttribute.abilityIcons.ContainsKey(abilityType))
                    {
                        content.image = AddAbilityMenuAttribute.abilityIcons[abilityType];
                        selected = GUILayout.Button(content, selectAbilityStyle, GUILayout.Height(40));
                    }
                    else
                    {
                        selected = GUILayout.Button(content, selectAbilityStyle);
                    }

                    if (selected)
                    {
                        SelectAbilityPopup popup = new SelectAbilityPopup();
                        popup.editor = this;
                        popup.data = abilityData;
                        PopupWindow.Show(selectAbilityRect, popup);
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    bool clicked = EditorUtils.Button(Color.red, "Remove Ability", EditorUtils.BoldButtonStyle());
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    if (!clicked)
                    {
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        var m = typeof(AbilityData).GetMethod("RemoveAbility", BindingFlags.Instance | BindingFlags.NonPublic);
                        MethodInfo removed = m.MakeGenericMethod(abilityType);
                        abilityEditors.Remove(abilityType);
                        removed.Invoke(abilityData, null);
                    }
                    GUILayout.FlexibleSpace();
                }
                #endregion
            }
        }
    }
}
