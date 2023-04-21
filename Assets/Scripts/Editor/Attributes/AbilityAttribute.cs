using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to add Abilities to the pool of available Abilities. This will allow the creations of these abilities inside AbilityData
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AbilityAttribute : Attribute
{
    public static HashSet<Type> actions = new HashSet<Type>();
    public AbilityAttribute(Type type) { 
        actions.Add(type);
    }
}
