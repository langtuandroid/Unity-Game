using System;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using System.Reflection;

namespace LobsterFramework.Editors
{
    public class AddWeaponStatPopup : PopupWindowContent
    {
        public WeaponData data;
        private Vector2 scrollPosition;

        public override void OnGUI(Rect rect)
        {
            if (data == null)
            {
                EditorGUILayout.LabelField("Cannot Find WeaponData!");
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            bool hasWeaponStat = false;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (Type type in AddWeaponStatMenuAttribute.types)
            {
                // Display icon in options if there's one for the ability script
                if (data.weaponStats.ContainsKey(type.ToString()))
                {
                    continue;
                }
                hasWeaponStat = true;
                GUIContent content = new();
                content.text = type.Name;
                if (AddWeaponStatMenuAttribute.icons.ContainsKey(type))
                {
                    content.image = AddWeaponStatMenuAttribute.icons[type];
                }
                if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.Width(180)))
                {
                    var m = typeof(WeaponData).GetMethod("AddWeaponStat", BindingFlags.Instance | BindingFlags.NonPublic);
                    MethodInfo mRef = m.MakeGenericMethod(type);
                    mRef.Invoke(data, null);
                }
            }
            GUILayout.EndScrollView();
            if (!hasWeaponStat)
            {
                GUIStyle color = new();
                color.normal.textColor = Color.yellow;
                EditorGUILayout.LabelField("No WeaponStat to add!", color);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
