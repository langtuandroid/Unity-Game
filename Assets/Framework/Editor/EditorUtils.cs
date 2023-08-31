using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LobsterFramework.Editors
{
    public class EditorUtils
    {
        #region Button
        public static bool Button(Color color, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Color before = GUI.color;
            GUI.color = color;
            bool result = GUILayout.Button(content, style, options);
            GUI.color = before;
            return result;
        }

        public static bool Button(Color color, GUIContent content, params GUILayoutOption[] options)
        {
            Color before = GUI.color;
            GUI.color = color;
            bool result = GUILayout.Button(content, options);
            GUI.color = before;
            return result;
        }

        public static bool Button(Color color, string content, GUIStyle style, params GUILayoutOption[] options)
        {
            Color before = GUI.color;
            GUI.color = color;
            bool result = GUILayout.Button(content, style, options);
            GUI.color = before;
            return result;
        }

        public static bool Button(Color color, string content, params GUILayoutOption[] options)
        {
            Color before = GUI.color;
            GUI.color = color;
            bool result = GUILayout.Button(content, options);
            GUI.color = before;
            return result;
        }
        #endregion

        public static GUIStyle BoldButtonStyle()
        {
            GUIStyle style = new(GUI.skin.button);
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        public static bool SetPropertyPointer(SerializedProperty property, string name) {
            property.Reset();
            while (property.name != name) {
                if (!property.Next(true)) {
                    return false;
                }
            }
            return true;
        }
    }
}
