using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ActionDataSODrawer : PropertyDrawer
{
    private SerializedObject obj = null;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            return EditorGUIUtility.singleLineHeight * 1.2f;
        }
        if (obj == null) {
            obj = new(property.objectReferenceValue);
        }
        obj.Update();
        return EditorGUI.GetPropertyHeight(obj.FindProperty("data"), label) + EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label, property.objectReferenceValue, typeof(ActionPrioritySO), true);
        if (property.objectReferenceValue == null)
        {
            return;
        }
        if (obj == null)
        {
            obj = new(property.objectReferenceValue);
        }
        obj.Update();
        Rect rect = new(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, obj.FindProperty("data"), true);
    }

}
