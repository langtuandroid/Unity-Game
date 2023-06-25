using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VarFloat))]
public class VarFloatDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null || !property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        return EditorGUIUtility.singleLineHeight * 2;

    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.objectReferenceValue = (VarFloat)EditorGUI.ObjectField(rect, label.text, property.objectReferenceValue, typeof(VarFloat), true);
        if (property.isExpanded && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            SerializedObject obj = new SerializedObject(property.objectReferenceValue);
            SerializedProperty child = obj.FindProperty("value");
            Rect rect1 = new(position.x, rect.y + EditorGUIUtility.singleLineHeight * 1.2f, position.width, rect.height);
            child.floatValue = EditorGUI.FloatField(rect1, "Value", child.floatValue);
            obj.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
