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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false, Inherited = false)] 
    public class ComponentRequiredAttribute : Attribute
    {
        private static Dictionary<Type, ClassNode> requirement = new();

        private Type[] components;

        public ComponentRequiredAttribute(params Type[] requiredComponents)
        {
            components = requiredComponents;
        }

        public void Init(Type type) { 
            InsertNode(type, components);
        }

        private static void InsertNode(Type abilityType, params Type[] requiredComponents) {
            if (!abilityType.IsSubclassOf(typeof(Ability)))
            {
                Debug.LogError("Type:" + abilityType.ToString() + " is not an Ability!");
                return;
            }
            if (requirement.ContainsKey(abilityType))
            {
                Debug.LogWarning("Ability:" + abilityType.ToString() + " is requiring components from multiple sources!");
                return;
            }
            ClassNode node = new ClassNode(abilityType);
            if (requiredComponents != null) {
                foreach (Type t in requiredComponents)
                {
                    if (!t.IsSubclassOf(typeof(Component)))
                    {
                        Debug.LogError("Cannot apply require Component of type:" + t.ToString() + " to " + abilityType.ToString()); 
                        return;
                    }
                    node.requirements.Add(t);
                }
            }
            
            foreach (Type t in requirement.Keys)
            {
                if (t.IsSubclassOf(abilityType))
                {
                    ClassNode child = requirement[t];
                    while (true)
                    {
                        if (child.type.IsSubclassOf(abilityType))
                        {
                            if (child.parent != null)
                            {
                                child = child.parent;
                            }
                            else
                            {
                                child.parent = node;
                                node.children.Add(child);
                                break;
                            }
                        }
                        else
                        {
                            child.children.Add(node);
                            node.parent = child;
                            break;
                        }
                    }
                }
                if (abilityType.IsSubclassOf(t))
                {
                    ClassNode parent = FindParent(node, requirement[t]);
                    parent.children.Add(node);
                    node.parent = parent;
                }
            }
            requirement[abilityType] = node;
        }

        private static ClassNode FindParent(ClassNode node, ClassNode parent) {
            if (node.type.IsSubclassOf(parent.type)) {
                foreach (ClassNode child in parent.children) { 
                    ClassNode p = FindParent(child, node);
                    if (p != null) {
                        return p;
                    }
                }
                return parent;
            }
            return null;
        }

        /// <summary>
        /// Check if the action component and the gameobject satisfies the requirements of the ActionInstance defined by its attributes.
        /// </summary>
        /// <param name="type">The type of the ability to examine for requirements</param>
        /// <param name="obj"> The gameobject for examination </param>
        /// <returns>Whether the gameobject satisfied the component requirements. If there's no requirement, return true. If obj is null, return false.</returns>
        public static bool ComponentCheck(Type type, GameObject obj) {
            if (!requirement.ContainsKey(type)) {
                InsertNode(type, default);
            }
            if (obj == null) {
                return false;
            }

            ClassNode node = requirement[type];
            List<Type> missing = new List<Type>();
            bool flag = true;
            while (node != null) {
                foreach (Type comType in node.requirements) {
                    if (obj.GetComponent(comType) == null) { flag = false; missing.Add(comType); }
                }
                node = node.parent;
            }
            if (!flag)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Missing Component: ");
                foreach (Type t in missing)
                {
                    sb.Append(t.Name);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                Debug.LogError(sb.ToString(), obj);
            }
            return flag;
        }
    }

    public class ClassNode
    {
        public Type type;
        public ClassNode parent = default;
        public List<ClassNode> children = new();
        public HashSet<Type> requirements = new();

        public ClassNode(Type type) { this.type = type; }
    }
}
