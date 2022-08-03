using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

public class AttributeInitializer
{
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Initialize() {
        foreach (Type type in typeof(Setting).Assembly.GetTypes())
        {
            type.GetCustomAttributes();
        }
    }
}
