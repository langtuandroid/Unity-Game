using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LobsterFramework.Editors
{
    [CustomPropertyDrawer(typeof(DisableInPlayMode))]
    public class DisableInPlayModeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
