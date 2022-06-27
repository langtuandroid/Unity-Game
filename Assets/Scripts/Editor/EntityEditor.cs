using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Entity entity = (Entity)target;
        if (GUILayout.Button("Test")) {
            Debug.Log("Testing");
        }
        EditorGUILayout.LabelField("Entities: ", entity.NumOfEntities().ToString());
    }
}
