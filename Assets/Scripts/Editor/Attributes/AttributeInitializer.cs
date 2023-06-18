using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.Linq;

public class AttributeInitializer
{
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Initialize() {
        foreach (Type type in typeof(GameManager).Assembly.GetTypes())
        {
            type.GetCustomAttributes();
        }
    }
}
