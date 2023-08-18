using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace LobsterFramework.AbilitySystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddAbilityStatMenuAttribute : Attribute
    {
        public static HashSet<Type> types = new HashSet<Type>();
        public static Dictionary<Type, Texture2D> icons = new();
        public void Init(Type type) {
            if (type.IsSubclassOf(typeof(AbilityStat)))
            {
                types.Add(type);
                MonoScript script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
                SerializedObject scriptObj = new(script);
                SerializedProperty iconProperty = scriptObj.FindProperty("m_Icon");
                Texture2D texture = (Texture2D)iconProperty.objectReferenceValue;
                if (texture != null)
                {
                    icons[type] = texture;
                }
            }
            else
            {
                Debug.LogError("Attempting to apply AbilitStat Attribute on invalid type: " + type.Name);
            }
        }
    }
}
