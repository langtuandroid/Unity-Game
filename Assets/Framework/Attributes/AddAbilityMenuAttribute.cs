using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LobsterFramework.AbilitySystem {

    /// <summary>
    /// Used to add Abilities to the pool of available Abilities. This will allow the creations of these abilities inside AbilityData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AddAbilityMenuAttribute : Attribute
    {
        public static HashSet<Type> abilities = new HashSet<Type>();
        public static Dictionary<Type, Texture2D> abilityIcons = new Dictionary<Type, Texture2D>();

        public void AddAbility(Type type) {
            if (type.IsSubclassOf(typeof(Ability)))
            {
                abilities.Add(type);
#if UNITY_EDITOR
                MonoScript script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
                try
                {
                    SerializedObject scriptObj = new(script);
                    SerializedProperty iconProperty = scriptObj.FindProperty("m_Icon");
                    Texture2D texture = (Texture2D)iconProperty.objectReferenceValue;
                    if (texture != null)
                    {
                        abilityIcons[type] = texture;
                    }
                }catch(NullReferenceException e)
                {
                    Debug.LogError("Null pointer exception when setting icon for script: " + type.FullName);
                }
#endif
            }
            else {
                Debug.LogError("The type specified for ability menu is not an ability:" + type.Name);
            }
        }
    }
}
