using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using LobsterFramework.Action;

namespace LobsterFramework.EditorUtility
{
    [CustomEditor(typeof(Actionable))]
    public class ActionEditor : Editor
    {
        private Editor editor;
        private bool editData = false;
        private string assetName = "";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Note: The action data may not work properly before the first run of the game. " +
                "Enter play mode for the first time to verify its integrity!", MessageType.Info);
            base.OnInspectorGUI();
            Actionable actionable = (Actionable)target;
            EditorGUI.BeginChangeCheck();
            SerializedProperty actionData = serializedObject.FindProperty("actionableData");
            if (!editData)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (Application.isPlaying && GUILayout.Button("Edit Actionable Data", GUILayout.Width(150)))
                {
                    editData = true;
                    editor = null;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (editData)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                actionData.isExpanded = EditorGUILayout.Foldout(actionData.isExpanded, "Actionable Data");
                if (actionData.isExpanded && actionData.objectReferenceValue != null)
                {
                    EditorGUI.indentLevel++;
                    if (editor == null)
                    {
                        editor = CreateEditor(actionData.objectReferenceValue);
                    }
                    editor.OnInspectorGUI();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Asset Name");
                assetName = EditorGUILayout.TextField(assetName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save Actionable Data", GUILayout.Width(150)))
                {
                    actionable.SaveActionableData(assetName);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
