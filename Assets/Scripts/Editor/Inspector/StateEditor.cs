using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(State))]
public class StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        State gameState = (State)target;
        
    }
}
