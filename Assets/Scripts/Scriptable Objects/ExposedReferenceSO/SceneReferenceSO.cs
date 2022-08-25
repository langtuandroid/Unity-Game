using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class SceneReferenceSO : ScriptableObject
{
    public void Awake()
    {
        Type currentType = GetType();
        FieldInfo[] fieldInfos = currentType.GetFields();
        foreach (FieldInfo info in fieldInfos) {
            ExposedFieldAttribute exposedFieldAttribute = info.GetCustomAttribute<ExposedFieldAttribute>();
            if (exposedFieldAttribute == null) {
                continue;
            }

        }
    }

    public void InitializeReference() { 
    
    }
}
