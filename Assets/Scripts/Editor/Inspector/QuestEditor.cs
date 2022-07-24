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
        Quest quest = (Quest)target;
        SerializedProperty useFailCondition = serializedObject.FindProperty("useFailCondition");
        SerializedProperty failCondition = serializedObject.FindProperty("failCondition");
        SerializedProperty failOperation = serializedObject.FindProperty("failOperation");
        if (useFailCondition.boolValue) { 
            failCondition.objectReferenceValue = (Condition)EditorGUILayout.ObjectField("Fail Condition", failCondition.objectReferenceValue, typeof(Condition) , false );
            failOperation.objectReferenceValue = (Operation)EditorGUILayout.ObjectField("Fail Operation", failOperation.objectReferenceValue, typeof(Operation), false);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
