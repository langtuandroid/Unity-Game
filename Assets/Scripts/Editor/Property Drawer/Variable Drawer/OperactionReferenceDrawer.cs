using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(OperationReference))]
public class OperactionReferenceDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (property.isExpanded) {
            return EditorGUIUtility.singleLineHeight * 4.5f;
        }
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty description = property.FindPropertyRelative("description");
        if (description.stringValue != "")
        {
            property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, description.stringValue);
        }
        else {
            property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, "Operation");
        }

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            Rect rect1 = new(position.x, position.y + foldOutRect.height, position.width, EditorGUIUtility.singleLineHeight);
            Rect rect2 = new(rect1.x, rect1.y + rect1.height, rect1.width, rect1.height);
            Rect rect3 = new(rect2.x, rect2.y + rect2.height, rect2.width, rect2.height);

            EditorGUI.PropertyField(rect1, description);

            SerializedProperty useCoroutine = property.FindPropertyRelative("useCoroutine");
            useCoroutine.boolValue = EditorGUI.Toggle(rect2, "Use Coroutine", useCoroutine.boolValue);

            if (useCoroutine.boolValue)
            {
                SerializedProperty var = property.FindPropertyRelative("coroutine");
                EditorGUI.PropertyField(rect3, var, true);
            }
            else
            {
                SerializedProperty con = property.FindPropertyRelative("operation");
                EditorGUI.PropertyField(rect3, con, true);
            }

            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
