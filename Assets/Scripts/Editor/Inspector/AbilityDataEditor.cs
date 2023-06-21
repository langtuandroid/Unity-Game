using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.EditorUtility
{
    [CustomEditor(typeof(AbilityData))]
    public class AbilityDataEditor : Editor
    {
        private Dictionary<Type, Editor> aiEditors = new();
        private Dictionary<Type, Editor> acEditors = new();
        private bool init = true;
        private GUIStyle style1 = new();
        private GUIStyle style2 = new();

        public override void OnInspectorGUI()
        {
            if (init) {
                style1.normal.textColor = Color.cyan;
                style1.fontStyle = FontStyle.Bold;
                style2.normal.textColor = Color.yellow;
                style2.fontStyle = FontStyle.Bold;
                init = false;
            }
            AbilityData abilityData = (AbilityData)target;
            MethodInfo removed = null;

            EditorGUILayout.HelpBox("Note: The configurations for abilities will not be available before " +
                "the first run of the game! Please verify the integrity of data " +
                "by running the game first before moving on to remove corrupted configs.", MessageType.Info);
            EditorGUI.BeginChangeCheck();

            SerializedProperty actComp = serializedObject.FindProperty("stats");
            SerializedProperty avAct = serializedObject.FindProperty("allAbilities");

            // Draw Action Component Section
            EditorGUILayout.BeginHorizontal();
            actComp.isExpanded = EditorGUILayout.Foldout(actComp.isExpanded, "Ability Stats: " + abilityData.stats.Values.Count);
            bool aButton = GUILayout.Button("Add AbilityStat");
            EditorGUILayout.EndHorizontal();

            if (aButton) // Add action component button clicked
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type type in AbilityStatAttribute.types)
                {
                    menu.AddItem(new GUIContent(type.Name), false,
                        () =>
                        {
                            var m = typeof(AbilityData).GetMethod("AddAbilityStat");
                            MethodInfo mRef = m.MakeGenericMethod(type);
                            mRef.Invoke(abilityData, null);
                        });
                }
                menu.ShowAsContext();
            }

            if (actComp.isExpanded)
            {
                EditorGUI.indentLevel++;
                int i = 0;
                foreach (AbilityStat component in abilityData.stats.Values)
                {
                    i += 1;
                    Editor editor;
                    Type type = component.GetType();
                    if (acEditors.ContainsKey(type))
                    {
                        editor = acEditors[type];
                    }
                    else
                    {
                        editor = CreateEditor(component);
                        acEditors.Add(type, editor);
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(component.GetType().Name, style1);
                    bool clicked = GUILayout.Button("Remove Stat");
                    EditorGUILayout.EndHorizontal();
                    if (!clicked)
                    {
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        var m = typeof(AbilityData).GetMethod("RemoveAbilityStat");
                        removed = m.MakeGenericMethod(component.GetType());
                        acEditors.Remove(type);
                    }
                    GUILayout.FlexibleSpace();
                    if (i != abilityData.stats.Count)
                    {
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    }
                }
                EditorGUI.indentLevel--;
                if (removed != null)
                {
                    removed.Invoke(abilityData, null);
                }
            }

            EditorGUILayout.Space();

            // Draw Action Instance Section
            EditorGUILayout.BeginHorizontal();
            avAct.isExpanded = EditorGUILayout.Foldout(avAct.isExpanded, "Abilities: " + abilityData.allAbilities.Count);
            bool aiButton = GUILayout.Button("Add Ability");
            EditorGUILayout.EndHorizontal();

            if (aiButton) // Add action component button clicked
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type type in AddAbilityMenuAttribute.actions)
                {
                    menu.AddItem(new GUIContent(type.Name), false,
                        () =>
                        {
                            var m = typeof(AbilityData).GetMethod("AddAbility");
                            MethodInfo mRef = m.MakeGenericMethod(type);
                            mRef.Invoke(abilityData, null);
                        });
                }
                menu.ShowAsContext();
            }

            if (avAct.isExpanded)
            {
                EditorGUILayout.HelpBox("Note: When editing list properties of abilities, drag reference directly to the list itself instead of its element fields, " +
                "otherwise the reference may not be saved.", MessageType.Info, true);
                removed = null;
                EditorGUI.indentLevel++;
                int i = 0;
                foreach (Ability action in abilityData.allAbilities.Values)
                {
                    i += 1;
                    Editor editor;
                    Type actionType = action.GetType();
                    if (!aiEditors.ContainsKey(actionType))
                    {
                        editor = CreateEditor(action);
                        aiEditors.Add(actionType, editor);
                    }
                    else
                    {
                        editor = aiEditors[actionType];
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(action.GetType().Name, style2);
                    bool clicked = GUILayout.Button("Remove Ability");
                    EditorGUILayout.EndHorizontal();
                    if (!clicked)
                    {
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        var m = typeof(AbilityData).GetMethod("RemoveAbility");
                        removed = m.MakeGenericMethod(actionType);
                        aiEditors.Remove(actionType);
                    }
                    GUILayout.FlexibleSpace();
                    if (i != abilityData.allAbilities.Count)
                    {
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    }
                }
                EditorGUI.indentLevel--;
                if (removed != null)
                {
                    removed.Invoke(abilityData, null);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
