using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ActionData))]
public class ActionDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if (property.objectReferenceValue != null && property.isExpanded) {
            SerializedObject obj = new(property.objectReferenceValue);
            height += EditorGUI.GetPropertyHeight(obj.FindProperty("priority"));
            height += EditorGUI.GetPropertyHeight(obj.FindProperty("cooldown"));
            height += EditorGUI.GetPropertyHeight(obj.FindProperty("duration"));
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        property.objectReferenceValue = (ActionData)EditorGUI.ObjectField(foldOutRect, label.text, property.objectReferenceValue, typeof(ActionData), true);

        if (property.isExpanded && property.objectReferenceValue != null) {
            EditorGUI.indentLevel++;
            // Priority
            SerializedObject obj = new(property.objectReferenceValue);
            SerializedProperty priority = obj.FindProperty("priority");
            Rect rect1 = new(position.x, foldOutRect.y + foldOutRect.height, position.width, EditorGUI.GetPropertyHeight(priority));
            EditorGUI.PropertyField(rect1, priority);
            // Cooldown
            SerializedProperty cooldown = obj.FindProperty("cooldown");
            Rect rect2 = new Rect(rect1.x, rect1.y + rect1.height, rect1.width, EditorGUI.GetPropertyHeight(cooldown));
            EditorGUI.PropertyField (rect2, cooldown);
            // Duration
            SerializedProperty duration = obj.FindProperty("duration");
            EditorGUI.PropertyField(new Rect(rect2.x, rect2.y + rect2.height, rect2.width, EditorGUI.GetPropertyHeight(duration)), duration);
            obj.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }
}
