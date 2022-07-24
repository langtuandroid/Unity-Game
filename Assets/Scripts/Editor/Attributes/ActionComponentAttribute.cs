using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class ActionComponentAttribute : Attribute
{
    public static HashSet<Type> types = new HashSet<Type>();
    public ActionComponentAttribute(Type type) {
        types.Add(type);
    }
}
