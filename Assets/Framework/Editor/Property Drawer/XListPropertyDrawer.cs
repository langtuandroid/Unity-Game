using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LobsterFramework.Utility;

namespace LobsterFramework.Editors
{
    [CustomPropertyDrawer(typeof(XList<>))]
    public class XListPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
        {
            property.Next(true);
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.Next(true);
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
            else {
                EditorGUI.PropertyField(position, property, label);
            }
            
        }
    }
}
