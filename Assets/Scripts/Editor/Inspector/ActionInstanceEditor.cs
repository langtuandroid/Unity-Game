using System.Linq;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.EditorUtility
{
    [CustomEditor(typeof(Ability), true)]
    public class ActionInstanceEditor : Editor
    {
        private string selectedConfig;
        private string addConfigName;
        private Editor editor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            Ability actionInstance = (Ability)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Config Name");
            addConfigName = EditorGUILayout.TextField(addConfigName);
            if (GUILayout.Button("Add"))
            {
                if (addConfigName == null)
                {
                    Debug.LogError("Field is empty!");
                }
                else
                {
                    actionInstance.AddConfig(addConfigName);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (actionInstance.configs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new();
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(selectedConfig, style);
                if (GUILayout.Button("Select Config"))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (string configName in actionInstance.configs.Keys)
                    {
                        menu.AddItem(new GUIContent(configName), false,
                            () =>
                            {
                                selectedConfig = configName;
                                editor = null;
                            });
                    }
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (selectedConfig != null)
            {
                if (editor == null)
                {
                    editor = CreateEditor(actionInstance.configs[selectedConfig]);
                }

                editor.OnInspectorGUI();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove Config", GUILayout.Width(150)))
                {
                    actionInstance.RemoveConfig(selectedConfig);
                    selectedConfig = null;
                    editor = null;
                }
                GUILayout.EndHorizontal();
            }
            else if (actionInstance.configs.Count > 0)
            {
                selectedConfig = actionInstance.configs.First().Key;
            }
            else
            {
                EditorGUILayout.LabelField("No configs available");
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
