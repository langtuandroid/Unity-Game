using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RefBool))]
public class RefBoolDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 1.3f;
        if (!property.isExpanded)
        {
            return height;
        }
        height *= 2;
        bool useSharedValue = property.FindPropertyRelative("useSharedValue").boolValue;
        if (useSharedValue)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sharedValue"));
        }
        else
        {
            height += EditorGUIUtility.singleLineHeight;
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty p1 = property.FindPropertyRelative("useSharedValue");
        SerializedProperty var = property.FindPropertyRelative("sharedValue");
        SerializedProperty con = property.FindPropertyRelative("value");

        bool value = new RefBool(con.boolValue, p1.boolValue, (VarBool)var.objectReferenceValue).Value;

        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, " ", value.ToString());

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float addY = EditorGUIUtility.singleLineHeight;
            Rect rect1 = new(position.x, position.y + addY, rect.width, EditorGUIUtility.singleLineHeight);
            p1.boolValue = EditorGUI.Toggle(rect1, "Use Shared Value", p1.boolValue);
            Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, rect1.height);
            if (p1.boolValue)
            {
                EditorGUI.PropertyField(rect2, var, true);
            }
            else
            {
                con.boolValue = EditorGUI.Toggle(rect2, "Value", con.boolValue);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
