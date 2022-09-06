using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateSO))]
public class StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StateSO gameState = (StateSO)target;
        
    }
}
