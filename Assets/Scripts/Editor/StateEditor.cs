using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(State))]
public class StateEditor : Editor
{
    private static string currentName = "";
    private static string setNameStatus = "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        State gameState = (State)target;
        EditorGUILayout.LabelField("Active: " + gameState.Active);
        EditorGUILayout.LabelField("State Name: " + gameState.GetName());
        EditorGUILayout.BeginHorizontal();
        currentName = EditorGUILayout.TextField(currentName, GUILayout.MaxWidth(150));
        if (GUILayout.Button("Confirm", GUILayout.MaxWidth(60)))
        {
            if (gameState.SetName(currentName))
            {
                setNameStatus = "Name successfully being set to: " + currentName;
            }
            else {
                setNameStatus = "Operation Failed, name conflict in the given state container!"; 
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField(setNameStatus);
    }
}
