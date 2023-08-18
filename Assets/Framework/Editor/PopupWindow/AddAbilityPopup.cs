using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using System;
using System.Reflection;

namespace LobsterFramework.Editors
{
    public class AddAbilityPopup : PopupWindowContent
    {
        public static AbilityData data;
        private Vector2 scrollPosition = Vector2.zero;

        public override void OnGUI(Rect rect)
        {
            if(data == null)
            {
                EditorGUILayout.LabelField("Cannot Find AbilityData!");
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            bool hasAbility = false;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (Type type in AddAbilityMenuAttribute.abilities)
            {
                // Display icon in options if there's one for the ability script
                if (data.allAbilities.ContainsKey(type.ToString()))
                {
                    continue;
                }
                hasAbility = true;
                GUIContent content = new();
                content.text = type.Name;
                if (AddAbilityMenuAttribute.abilityIcons.ContainsKey(type))
                {
                    content.image = AddAbilityMenuAttribute.abilityIcons[type];
                }
                if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.Width(180)))
                {
                    var m = typeof(AbilityData).GetMethod("AddAbility", BindingFlags.Instance | BindingFlags.NonPublic);
                    MethodInfo mRef = m.MakeGenericMethod(type);
                    mRef.Invoke(data, null);
                }
            }
            GUILayout.EndScrollView();
            if (!hasAbility) {
                GUIStyle color = new();
                color.normal.textColor = Color.yellow;
                EditorGUILayout.LabelField("No Abilitys to add!", color);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
