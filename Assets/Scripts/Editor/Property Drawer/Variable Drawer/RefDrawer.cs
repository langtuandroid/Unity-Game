using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RefDrawer<T> : PropertyDrawer where T : ScriptableObject
{
    protected T GetInstance(SerializedProperty property)
    {
        if (property.objectReferenceValue != null) {
            return (T)property.objectReferenceValue;
        }
        T m_obj = ScriptableObject.CreateInstance<T>();
        if (AssetDatabase.Contains(property.serializedObject.targetObject))
        {
            AssetDatabase.AddObjectToAsset(m_obj, property.serializedObject.targetObject);
        }
        property.objectReferenceValue = m_obj;
        return m_obj;
    }
}
