using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RefActionData))]
public class RefActionDataDrawer : RefDrawer<RefActionData>
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
                SerializedProperty p = obj.FindProperty("value");
                if (p.objectReferenceValue == null) {
                    p.objectReferenceValue = ScriptableObject.CreateInstance<ActionData>(); 
                    obj.ApplyModifiedProperties();
                }
                SerializedObject obj2 = new(p.objectReferenceValue);
                height += EditorGUI.GetPropertyHeight(obj2.FindProperty("priority"));
                height += EditorGUI.GetPropertyHeight(obj2.FindProperty("cooldown"));
                height += EditorGUI.GetPropertyHeight(obj2.FindProperty("duration"));
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RefActionData m_obj = GetInstance(property);

        EditorGUI.BeginProperty(position, label, property);
        // Foldout
        Rect foldOutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label);

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, label.text);

        if (property.isExpanded && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            float addY = EditorGUIUtility.singleLineHeight;
            SerializedObject obj = new(m_obj);
            SerializedProperty p1 = obj.FindProperty("useSharedValue");
            Rect rect1 = new(position.x, position.y + addY, rect.width, EditorGUIUtility.singleLineHeight);
            p1.boolValue = EditorGUI.Toggle(rect1, "Use Shared Value", p1.boolValue);
            if (p1.boolValue)
            {
                Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, rect1.height);
                SerializedProperty var = obj.FindProperty("sharedValue");
                EditorGUI.PropertyField(rect2, var, true);
            }
            else
            {
                SerializedProperty p = obj.FindProperty("value");
                ActionData data;
                if (p.objectReferenceValue == null)
                {
                    data = ScriptableObject.CreateInstance<ActionData>();
                    p.objectReferenceValue = data;
                }
                else {
                    data = (ActionData)p.objectReferenceValue;
                }
                SerializedObject p_obj = new(data);
                // Priority
                SerializedProperty priority = p_obj.FindProperty("priority");
                Rect rect2 = new(rect1.x, rect1.y + addY, rect1.width, EditorGUI.GetPropertyHeight(priority));
                EditorGUI.PropertyField(rect2, priority);
                // Cooldown
                SerializedProperty cooldown = p_obj.FindProperty("cooldown");
                Rect rect3 = new Rect(rect2.x, rect2.y + rect2.height, rect2.width, EditorGUI.GetPropertyHeight(cooldown));
                EditorGUI.PropertyField(rect3, cooldown);
                // Duration
                SerializedProperty duration = p_obj.FindProperty("duration");
                EditorGUI.PropertyField(new Rect(rect3.x, rect3.y + rect3.height, rect3.width, EditorGUI.GetPropertyHeight(duration)), duration);
                p_obj.ApplyModifiedProperties();
            }

            obj.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
