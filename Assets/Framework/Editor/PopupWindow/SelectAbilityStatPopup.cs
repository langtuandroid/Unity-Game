using System;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.Editors
{
    public class SelectAbilityStatPopup : PopupWindowContent
    {
        public AbilityDataEditor editor;
        public AbilityData data;
        private Vector2 scrollPosition;

        public override void OnGUI(Rect rect)
        {
            if (data == null || editor == null)
            {
                EditorGUILayout.LabelField("Cannot Find AbilityData or Editor!");
                return;
            }
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (Type type in AddAbilityStatMenuAttribute.types)
            {
                // Display icon in options if there's one for the ability script
                string key = type.ToString();
                if (!data.stats.ContainsKey(key))
                {
                    continue;
                }
                GUIContent content = new();
                content.text = type.Name;
                if (AddAbilityStatMenuAttribute.icons.ContainsKey(type))
                {
                    content.image = AddAbilityStatMenuAttribute.icons[type];
                }
                if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.Width(180)))
                {
                    editor.selectedAbilityStat = data.stats[key];
                    editorWindow.Close();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
