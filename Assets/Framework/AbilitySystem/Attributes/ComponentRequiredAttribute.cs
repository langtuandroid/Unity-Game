using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// Mark this ability as requiring the specified components on the parent gameobject to run
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)] 
    public class ComponentRequiredAttribute : Attribute
    {
        private static Dictionary<Type, List<Type>> requirement = new();

        private Type[] components;

        public ComponentRequiredAttribute(params Type[] requiredComponents)
        {
            components = requiredComponents;
        }

        public void Init(Type type) {
            if (!requirement.ContainsKey(type)) {
                requirement[type] = new List<Type>();
            }
            foreach(Type component in components)
            {
                requirement[type].Add(component);
            }
        }

        /// <summary>
        /// Check if the action component and the gameobject satisfies the requirements of the ActionInstance defined by its attributes.
        /// </summary>
        /// <param name="type">The type of the ability to examine for requirements</param>
        /// <param name="objects"> The gameobject for examination </param>
        /// <returns>Whether the gameobject satisfied the component requirements. If there's no requirement, return true. If obj is null, return false.</returns>
        public static bool ComponentCheck(Type type, params GameObject[] objects) {
            if (!requirement.ContainsKey(type)) {
                return true;
            }
            List<Type> missing = new List<Type>();
            foreach (Type component in requirement[type])
            {
                bool result = false;
                foreach(GameObject obj in objects)
                {
                    if (obj.GetComponent(component) != null) {
                        result = true; break;
                    }
                }
                if (!result) {
                    missing.Add(component);
                }
            }

            if(missing.Count > 0) {
                string debugString = "Missing Components: ";
                foreach (Type component in missing)
                {
                    debugString += component.ToString() + ", ";
                }
                debugString = debugString.Remove(debugString.Length - 2);
                Debug.LogError(debugString);
                return false;
            }
            
            return true;
        }
    }
}
