using System;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.Editors
{
    public class SelectWeaponStatPopup : PopupWindowContent
    {
        public WeaponDataEditor editor;
        public WeaponData data;
        private Vector2 scrollPosition;

        public override void OnGUI(Rect rect)
        {
            if (data == null || editor == null)
            {
                EditorGUILayout.LabelField("Cannot Find WeaponData or Editor!");
                return;
            }
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (Type type in AddWeaponStatMenuAttribute.types)
            {
                // Display icon in options if there's one for the WeaponStat script
                string key = type.ToString();
                if (!data.weaponStats.ContainsKey(key))
                {
                    continue;
                }
                GUIContent content = new();
                content.text = type.Name;
                if (AddWeaponStatMenuAttribute.icons.ContainsKey(type))
                {
                    content.image = AddWeaponStatMenuAttribute.icons[type];
                }
                if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.Width(180)))
                {
                    editor.selectedWeaponStat = data.weaponStats[key];
                    editorWindow.Close();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
