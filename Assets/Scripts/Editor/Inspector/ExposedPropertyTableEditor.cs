using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExposedPropertyTable))]
public class ExposedPropertyTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox("Warning: Use '+' to add element will ignore duplication check, use 'Add Element' instead.", MessageType.Warning);
        SerializedProperty lst = serializedObject.FindProperty("infos");
        EditorGUILayout.PropertyField(lst);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        SerializedProperty add = serializedObject.FindProperty("addElement");
        EditorGUILayout.PropertyField(add);
        if (GUILayout.Button("Add")) {
            ExposedPropertyTable table = (ExposedPropertyTable)target;
            table.AddObjectReference();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
