using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddWeaponStatMenuAttribute : Attribute
    {
        public static HashSet<Type> types = new HashSet<Type>();
        public static Dictionary<Type, Texture2D> icons = new();
        public void Init(Type type)
        {
            if (type.IsSubclassOf(typeof(WeaponStat)))
            {
                types.Add(type);
#if UNITY_EDITOR
                MonoScript script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
                try
                {
                    SerializedObject scriptObj = new(script);
                    SerializedProperty iconProperty = scriptObj.FindProperty("m_Icon");
                    Texture2D texture = (Texture2D)iconProperty.objectReferenceValue;
                    if (texture != null)
                    {
                        icons[type] = texture;
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.LogError("Null pointer exception when setting icon for script: " + type.FullName);
                }
#endif
            }
            else
            {
                Debug.LogError("Attempting to apply WeaponStat Attribute on invalid type: " + type.Name);
            }
        }
    }
}
