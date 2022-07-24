using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RefBool))]
public class RefBoolDrawer : RefDrawer<RefBool>
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 1.2f;
        if (!property.isExpanded)
        {
            return height;
        }
        if (property.objectReferenceValue != null)
        {
            height *= 2;
            SerializedObject obj = new SerializedObject(property.objectReferenceValue);
            bool useSharedValue = obj.FindProperty("useSharedValue").boolValue;
            if (useSharedValue)
            {
                height += EditorGUI.GetPropertyHeight(obj.FindProperty("sharedValue"));
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RefBool m_obj = GetInstance(property);

        EditorGUI.BeginProperty(position, label, property);

        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, label.text, m_obj.Value.ToString());

        if (property.isExpanded && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            float addY = EditorGUIUtility.singleLineHeight;
            SerializedObject obj = new(m_obj);
            SerializedProperty p1 = obj.FindProperty("useSharedValue");
            Rect rect1 = new(position.x, position.y + addY, rect.width, EditorGUIUtility.singleLineHeight);
            p1.boolValue = EditorGUI.Toggle(rect1, "Use Shared Value", p1.boolValue);
            Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, rect1.height);
            if (p1.boolValue)
            {
                SerializedProperty var = obj.FindProperty("sharedValue");
                EditorGUI.PropertyField(rect2, var, true);
            }
            else
            {
                SerializedProperty con = obj.FindProperty("value");
                con.boolValue = EditorGUI.Toggle(rect2, "Value", con.boolValue);
            }

            obj.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
