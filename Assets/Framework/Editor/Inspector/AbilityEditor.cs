using System.Linq;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using System.Reflection;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Ability), true)]
    public class AbilityEditor : UnityEditor.Editor
    {
        private string selectedConfig;
        private string addConfigName;
        private Editor editor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            Ability ability = (Ability)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Config Name");
            GUILayout.FlexibleSpace();
            addConfigName = EditorGUILayout.TextField(addConfigName);
            

            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                if (addConfigName == null)
                {
                    Debug.LogError("Field is empty!");
                }
                else
                {
                    ability.AddConfig(addConfigName);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (ability.configs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new();
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(selectedConfig, style);
                if (GUILayout.Button("Select Config", GUILayout.Width(100)))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (string configName in ability.configs.Keys)
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
                    editor = CreateEditor(ability.configs[selectedConfig]);
                }

                editor.OnInspectorGUI();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (EditorUtils.Button(Color.red, "Remove Config", EditorUtils.BoldButtonStyle(), GUILayout.Width(100)))
                {
                    ability.RemoveConfig(selectedConfig);
                    selectedConfig = null;
                    editor = null;
                }
                GUILayout.EndHorizontal();
            }
            else if (ability.configs.Count > 0)
            {
                selectedConfig = ability.configs.First().Key;
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
