using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RefAbilityPriority))]
public class RefActionPriorityDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 1.3f;
        if (!property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        height *= 2;
        bool useSharedValue = property.FindPropertyRelative("useSharedValue").boolValue;
        if (useSharedValue)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sharedValue"));
        }
        else
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty p1 = property.FindPropertyRelative("useSharedValue");
        SerializedProperty var = property.FindPropertyRelative("sharedValue");
        SerializedProperty con = property.FindPropertyRelative("value");

        EditorGUI.BeginProperty(position, label, property);
        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, label.text);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float addY = EditorGUIUtility.singleLineHeight;
            Rect rect1 = new(position.x, position.y + addY, rect.width, EditorGUIUtility.singleLineHeight);
            p1.boolValue = EditorGUI.Toggle(rect1, "Use Shared Value", p1.boolValue);
            
            if (p1.boolValue)
            {
                Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, EditorGUI.GetPropertyHeight(var));
                EditorGUI.PropertyField(rect2, var, true);
            }
            else
            {
                Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, EditorGUI.GetPropertyHeight(con));
                EditorGUI.PropertyField(rect2, con, true);
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
