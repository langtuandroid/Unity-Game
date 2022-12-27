using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(ActionableData))]
public class ActionableDataEditor : Editor
{

    public override void OnInspectorGUI()
    {
        ActionableData actionableData = (ActionableData)target;
        MethodInfo removed = null;

        EditorGUI.BeginChangeCheck();

        SerializedProperty actComp = serializedObject.FindProperty("components");
        SerializedProperty avAct = serializedObject.FindProperty("allActions");

        // Draw Action Component Section
        EditorGUILayout.BeginHorizontal();
        actComp.isExpanded = EditorGUILayout.Foldout(actComp.isExpanded, "Action Components: " + actionableData.components.Values.Count);
        bool aButton = GUILayout.Button("Add Action Component");
        EditorGUILayout.EndHorizontal();

        if (aButton) // Add action component button clicked
        {
            GenericMenu menu = new GenericMenu();
            foreach (Type type in ActionComponentAttribute.types)
            {
                menu.AddItem(new GUIContent(type.ToString()), false,
                    () =>
                    {
                        var m = typeof(ActionableData).GetMethod("AddActionComponent");
                        MethodInfo mRef = m.MakeGenericMethod(type);
                        mRef.Invoke(actionableData, null);
                    });
            }
            menu.ShowAsContext();
        }

        if (actComp.isExpanded)
        {
            EditorGUI.indentLevel++;
            int i = 0;
            foreach (ActionComponent component in actionableData.components.Values)
            {
                i += 1;
                Editor editor = CreateEditor(component);
                Type type = component.GetType();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(component.GetType().ToString(), EditorStyles.boldLabel);
                bool clicked = GUILayout.Button("Remove Component");
                EditorGUILayout.EndHorizontal();
                if (!clicked)
                {
                    editor.OnInspectorGUI();
                }
                else
                {
                    var m = typeof(ActionableData).GetMethod("RemoveActionComponent");
                    removed = m.MakeGenericMethod(component.GetType());
                }
                GUILayout.FlexibleSpace();
                if (i != actionableData.components.Count)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUI.indentLevel--;
            if (removed != null)
            {
                removed.Invoke(actionableData, null);
            }
        }

        EditorGUILayout.Space();

        // Draw Action Instance Section
        EditorGUILayout.BeginHorizontal();
        avAct.isExpanded = EditorGUILayout.Foldout(avAct.isExpanded, "Action Instances: " + actionableData.allActions.Count);
        bool aiButton = GUILayout.Button("Add Action Instance");
        EditorGUILayout.EndHorizontal();

        if (aiButton) // Add action component button clicked
        {
            GenericMenu menu = new GenericMenu();
            foreach (Type type in ActionInstanceAttribute.actions)
            {
                menu.AddItem(new GUIContent(type.ToString()), false,
                    () =>
                    {
                        var m = typeof(ActionableData).GetMethod("AddActionInstance");
                        MethodInfo mRef = m.MakeGenericMethod(type);
                        mRef.Invoke(actionableData, null);
                    });
            }
            menu.ShowAsContext();
        }

        if (avAct.isExpanded)
        {
            EditorGUILayout.HelpBox("Note: When editing list properties of action instances, drag reference directly to the list itself instead of its element fields, " +
            "otherwise the reference may not be saved.", MessageType.Info, true);
            removed = null;
            EditorGUI.indentLevel++;
            int i = 0;
            foreach (ActionInstance action in actionableData.allActions.Values)
            {
                i += 1;
                Editor editor = CreateEditor(action);
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new();
                style.normal.textColor = Color.yellow;
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(action.GetType().ToString(), style);
                bool clicked = GUILayout.Button("Remove Action");
                EditorGUILayout.EndHorizontal();
                if (!clicked)
                {
                    editor.OnInspectorGUI();
                }
                else
                {
                    var m = typeof(ActionableData).GetMethod("RemoveActionInstance");
                    removed = m.MakeGenericMethod(action.GetType());
                }
                GUILayout.FlexibleSpace();
                if (i != actionableData.allActions.Count)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUI.indentLevel--;
            if (removed != null)
            {
                removed.Invoke(actionableData, null);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
