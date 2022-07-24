using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class ActionInstanceAttribute : Attribute
{
    public static HashSet<Type> actions = new HashSet<Type>();
    public ActionInstanceAttribute(Type type) { 
        actions.Add(type);
    }
}
