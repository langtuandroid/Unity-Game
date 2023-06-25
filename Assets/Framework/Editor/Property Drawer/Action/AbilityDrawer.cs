using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.EditorUtility
{
    [CustomPropertyDrawer(typeof(Ability))]
    public class ActionInstanceDrawer : PropertyDrawer
    {
        private static Dictionary<System.Type, PropertyDrawer> drawers;

        public ActionInstanceDrawer()
        {
            drawers = new Dictionary<System.Type, PropertyDrawer>();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.objectReferenceValue == null)
            {
                return height;
            }

            System.Type type = property.objectReferenceValue.GetType();
            if (drawers.ContainsKey(type))
            {
                return drawers[type].GetPropertyHeight(property, label);
            }

            if (property.isExpanded)
            {
                SerializedObject o = new(property.objectReferenceValue);
                height += EditorGUI.GetPropertyHeight(o.FindProperty("actionData"), label);
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
            {
                return;
            }

            System.Type type = property.objectReferenceValue.GetType();
            if (drawers.ContainsKey(type))
            {
                drawers[type].OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(foldOutRect, label.text);

            property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label.text + ": " + type.ToString());

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                SerializedObject obj = new(property.objectReferenceValue);

                // Action Data
                SerializedProperty data = obj.FindProperty("actionData");
                Rect rect2 = new(foldOutRect.x, foldOutRect.y + foldOutRect.height, foldOutRect.width, EditorGUI.GetPropertyHeight(data));
                EditorGUI.PropertyField(rect2, data);

                obj.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
