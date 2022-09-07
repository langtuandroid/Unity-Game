using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(Actionable))]
public class ActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Actionable actionable = (Actionable)target;
        EditorGUI.BeginChangeCheck();
        SerializedProperty actionData = serializedObject.FindProperty("actionableData");
        if (GUILayout.Button("Set Actionable Data")) {
            actionable.SetActionableData();
        }

        if (actionData != null) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            actionData.isExpanded = EditorGUILayout.Foldout(actionData.isExpanded, "Actionable Data");
            if (actionData.isExpanded && actionData.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                Editor editor = Editor.CreateEditor(actionData.objectReferenceValue);
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
            if (GUILayout.Button("Save Actionable Data")) {
                actionable.SaveActionableData();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
