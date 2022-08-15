using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();
        SerializedProperty useFailCondition = serializedObject.FindProperty("useFailCondition");
        
        if (useFailCondition.boolValue) {
            SerializedProperty failOperation = serializedObject.FindProperty("failOperation");
            SerializedProperty failCondition = serializedObject.FindProperty("failCondition");
            EditorGUILayout.PropertyField(failCondition);
            EditorGUILayout.PropertyField(failOperation);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
