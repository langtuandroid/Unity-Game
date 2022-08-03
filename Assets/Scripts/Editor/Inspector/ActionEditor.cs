using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(Actionable))]
public class ActionComponentEditor : Editor
{
    bool acFoldout = false;
    bool aiFoldout = false;

    public override void OnInspectorGUI()
    {
        Actionable actionable = (Actionable)target;
        MethodInfo removed = null;

        // Action Queue
        SerializedProperty property = serializedObject.FindProperty("actionQueue");
        property.objectReferenceValue = (ActionQueue)EditorGUILayout.ObjectField("Action Queue", property.objectReferenceValue, typeof(ActionQueue), true);

        // Draw Action Component Section
        EditorGUILayout.BeginHorizontal();
        acFoldout =  EditorGUILayout.Foldout(acFoldout, "Action Components: " + actionable.components.Values.Count);
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
                        var m = typeof(Actionable).GetMethod("AddActionComponent");
                        MethodInfo mRef = m.MakeGenericMethod(type);
                        mRef.Invoke(actionable, null);
                    });
            }
            menu.ShowAsContext();
        }

        if (acFoldout) {
            EditorGUI.indentLevel++;
            int i = 0;
            foreach (ActionComponent component in actionable.components.Values)
            {
                i += 1;
                Editor editor = Editor.CreateEditor(component);
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
                    var m = typeof(Actionable).GetMethod("RemoveActionComponent");
                    removed = m.MakeGenericMethod(component.GetType());
                }
                GUILayout.FlexibleSpace();
                if (i != actionable.components.Count)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUI.indentLevel--;
            if (removed != null)
            {
                removed.Invoke(actionable, null);
            }
        }

        EditorGUILayout.Space();

        // Draw Action Instance Section
        EditorGUILayout.BeginHorizontal();
        aiFoldout = EditorGUILayout.Foldout(aiFoldout, "Action Instances: " + actionable.availableActions.Count);
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
                        var m = typeof(Actionable).GetMethod("AddActionInstance");
                        MethodInfo mRef = m.MakeGenericMethod(type);
                        mRef.Invoke(actionable, null);
                    });
            }
            menu.ShowAsContext();
        }

        if (aiFoldout) {
            removed = null;
            EditorGUI.indentLevel++;
            int i = 0;
            foreach (ActionInstance action in actionable.availableActions.Values)
            {
                i += 1;
                Editor editor = Editor.CreateEditor(action);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(action.GetType().ToString(), EditorStyles.boldLabel);
                bool clicked = GUILayout.Button("Remove Action");
                EditorGUILayout.EndHorizontal();
                if (!clicked)
                {
                    editor.OnInspectorGUI();
                }
                else
                {
                    var m = typeof(Actionable).GetMethod("RemoveActionInstance");
                    removed = m.MakeGenericMethod(action.GetType());
                }
                GUILayout.FlexibleSpace();
                if (i != actionable.availableActions.Count) {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
            EditorGUI.indentLevel--;
            if (removed != null)
            {
                removed.Invoke(actionable, null);
            }
        }

        if (serializedObject.ApplyModifiedProperties()) {
            Debug.Log("Modified");
        }
    }
}
